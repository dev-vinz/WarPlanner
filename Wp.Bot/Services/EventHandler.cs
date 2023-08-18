using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Wp.Bot.Modules.TimeEvents;
using Wp.Bot.Services.Logger.Event;
using Wp.Common.Models;
using Wp.Common.Services;
using Wp.Common.Settings;
using Wp.Database.Services;
using Wp.Discord;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Services
{
    public class EventHandler
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly DiscordSocketClient client;
        private readonly EventLogger logger;

        private bool isDatabaseVerified;
        private Thread threadEvent;
        private System.Timers.Timer eventTimer;
        private DateTimeOffset startDate;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */

        public TimeSpan Uptime => DateTimeOffset.UtcNow - startDate;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public EventHandler(DiscordSocketClient client)
        {
            // Inputs
            {
                this.client = client;
            }

            // Tools
            {
                logger = new EventLogger(client);
                isDatabaseVerified = false;
                threadEvent = null!;
                eventTimer = null!;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public async Task InitializeAsync()
        {
            // Initialize client with time loop when ready
            client.Ready += HandleTime;
            client.Ready += OnReadyAsync;

            // Initialize bots events
            client.JoinedGuild += GuildJoinedAsync;
            client.LeftGuild += GuildLeftAsync;

            await PrepareClientForEventsAsync();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private async Task<bool> GuildLeftAsync(ulong id)
        {
            LogMessage logMessage = new(LogSeverity.Info, "Gateway", $"Guild with id {id} left");
            Console.WriteLine(logMessage.ToString());

            await logger.GuildLeftAsync(id);

            // Guild left, DELETE CASCADE - We juste have to delete guild from DB
            return Database.Context.Guilds.Remove(g => g.Id == id);
        }

        private async Task GuildLeftAsync(SocketGuild guild)
        {
            await logger.GuildLeftAsync(guild);

            // Guild left, DELETE CASCADE - We juste have to delete guild from DB
            Database.Context.Guilds.Remove(g => g.Id == guild.Id);
        }

        private async Task<bool> GuildJoinedAsync(SocketGuild guild)
        {
            DbGuilds guilds = Database.Context.Guilds;

            // In doupts, checks that guild doesn't exists
            if (guilds.Any(g => g.Id == guild.Id)) return false;

            await logger.GuildJoinedAsync(guild);

            // Creates and adds new guild
            Guild newGuild = new(guild.Id, DefaultParameters.DEFAULT_TIME_ZONE);
            guilds.Add(newGuild);

            // Adds classic time actions too
            DbTimes times = Database.Context.Times;

            Time endWar = new(newGuild, TimeAction.DETECT_END_WAR, DateTimeOffset.UtcNow, Time.DETECT_END_WAR_INTERVAL, DefaultParameters.DEFAULT_TIME_ADDITIONAL);
            Time warStatus = new(newGuild, TimeAction.REMIND_WAR_STATUS, DateTimeOffset.UtcNow, Time.REMIND_WAR_STATUS_INTERVAL, DefaultParameters.DEFAULT_TIME_ADDITIONAL);

            times.Add(endWar);
            times.Add(warStatus);

            // Gets responses
            IGeneralResponse generalResponses = newGuild.GeneralResponses;
            ITime timeResponses = newGuild.TimeText;

            DateTimeOffset guildNow = newGuild.Now;
            TimeSpan offset = guildNow.Offset;

            // Documentation button
            ButtonBuilder docButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.Documentation)
                .WithStyle(ButtonStyle.Link)
                .WithUrl(Utilities.GITBOOK_DOCUMENTATION)
                .WithEmote(new Emoji("📚"));

            // Support button
            ButtonBuilder supportButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.SupportServer)
                .WithStyle(ButtonStyle.Link)
                .WithUrl(Utilities.SUPPORT_GUILD_INVITATION)
                .WithEmote(new Emoji("⚙️"));

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithButton(supportButtonBuilder)
                .WithButton(docButtonBuilder);

            IEnumerable<IGuildUser> users = await guild.GetUsersAsync().FlattenAsync();
            IGuildUser? owner = users.FirstOrDefault(u => u.Id == guild.OwnerId);

            if (owner == null) return true;

            // Embed
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(timeResponses.GuildJoinedTitle)
                .WithRandomColor()
                .WithDescription($"{CustomEmojis.WarPlanner} | {timeResponses.GuildJoinedDescription(owner.Username)}")
                .AddField(timeResponses.GuildJoinedFieldTimeZoneTitle, timeResponses.GuildJoinedFieldTimeZoneDescription(newGuild.TimeZone.AsAttribute().DisplayName, offset.TotalHours), true)
                .WithFooter($"{guildNow.Year} © {guild.CurrentUser.Username}");

            try
            {
                await owner.SendMessageAsync(embed: embed.Build(), components: componentBuilder.Build());
            }
            catch (Exception)
            {
                IReadOnlyCollection<SocketTextChannel> allChannels = guild.TextChannels;

                foreach (SocketTextChannel channel in guild.TextChannels)
                {
                    RestUserMessage msg = await channel.SendMessageAsync(owner.Mention, embed: embed.Build(), components: componentBuilder.Build());

                    if (msg != null) break;
                }
            }

            return true;
        }

        private Task HandleTime()
        {
            // Have to be in an other thread
            threadEvent = new(async () =>
            {
                // Until database is verified and operational
                while (!isDatabaseVerified)
                {
                    // Wait
                    Thread.Sleep(10);
                }

                eventTimer = JSFunctions.SetInterval(async () =>
                {
                    try
                    {
                        DbTimes times = Database.Context.Times;

                        // In each guild	
                        client.Guilds
                            .AsParallel()
                            .ForAll(guild =>
                            {
                                Time[] events = times.Where(t => t.Guild.Id == guild.Id).ToArray();

                                // For each time events
                                events
                                    .AsParallel()
                                    .ForAll(@event => TimeCaller.Execute(@event.Action, guild));
                            });
                    }
                    catch (Exception e)
                    {
                        LogMessage logMessage = new(LogSeverity.Info, "TimeLoop", "Error while executing time loop");
                        Console.WriteLine(logMessage.ToString());

                        await logger.EventExecutionFailedAsync(e);

                        JSFunctions.ClearInterval(ref eventTimer);

                        await HandleTime();
                    }
                }, TimeSpan.FromSeconds(15));

                LogMessage logMessage = new(LogSeverity.Info, "Discord", "Time loop started");
                Console.WriteLine(logMessage.ToString());

                await logger.EventExecutionStartedAsync();
            });

            threadEvent.Start();

            return Task.CompletedTask;
        }

        private async Task OnReadyAsync()
        {
            await logger.ClientReadyAsync();
            startDate = DateTimeOffset.UtcNow;
        }

        private async Task PrepareClientForEventsAsync()
        {
            DbGuilds guilds = Database.Context.Guilds;

            int cptJoined = 0;
            int cptLeft = 0;

            // Checks for existing and non existing guild in DB
            foreach (SocketGuild guild in client.Guilds)
            {
                // Checks if there's missing guilds
                bool joined = await GuildJoinedAsync(guild);

                if (joined) cptJoined += 1;
            }

            // Checks if we have to remove some guilds in DB
            foreach (Guild guild in guilds)
            {
                if (!client.Guilds.Any(g => g.Id == guild.Id))
                {
                    bool left = await GuildLeftAsync(guild.Id);

                    if (left) cptLeft += 1;
                }
            }

            LogMessage logMessage = new(LogSeverity.Info, "Database", $"Verified and operational (+{cptJoined}/-{cptLeft})");
            Console.WriteLine(logMessage.ToString());

            isDatabaseVerified = true;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



    }
}
