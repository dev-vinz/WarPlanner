using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using Wp.Api;
using Wp.Bot.Modules.ApplicationCommands.AutoCompletion;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;
using Calendar = Wp.Common.Models.Calendar;

namespace Wp.Bot.Modules.ApplicationCommands.Manager
{
    [Group("war", "War commands handler")]
    public class War : InteractionModuleBase<SocketInteractionContext>
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly CommandHandler handler;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public InteractionService? Commands { get; set; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public War(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [SlashCommand("add", "Register a new war within the calendar", runMode: RunMode.Async)]
        public async Task Add([Summary("opponent")] string opponentTag,
                              [Summary("date", "When the war should be"), Autocomplete(typeof(WarDateAutocompleteHandler))] string date,
                              [Summary("preparation", "How long the preparation time should be"), Autocomplete(typeof(WarPreparationAutocompleteHandler))] int warPreparation,
                              [Summary("war", "How long the war time should be"), Autocomplete(typeof(WarDurationAutocompleteHandler))] int warDuration)
        {
            await DeferAsync(true);

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            DbCompetitions competitions = Database.Context.Competitions;
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Calendar dbCalendar = calendars
                .AsParallel()
                .First(c => c.Guild == dbGuild);

            Common.Models.Competition[] dbCompetitions = competitions
                .AsParallel()
                .Where(c => c.Guild == dbGuild)
                .ToArray();

            Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .ToArray();

            // Gets command responses
            IAdmin adminResponses = dbGuild.AdminText;
            IManager commandText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Recovers parameters
            int totalTime = warPreparation + warDuration;
            DateTimeOffset warDate = DateTimeOffset.Parse(date);
            CultureInfo cultureInfo = dbGuild.Language.GetCultureInfo();

            // Gets Clash of Clans opponent clan
            ClashOfClans.Models.Clan? cOpponent = await ClashOfClansApi.Clans.GetByTagAsync(opponentTag);

            if (cOpponent is null)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

                return;
            }

            if (dbCalendar is null)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = adminResponses.CalendarIdNotSet);

                return;
            }

            if (!dbCompetitions.Any())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.WarAddNoCompetition);

                return;
            }

            // Gets all guild members
            IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

            // Filters one more time
            Player[] availablePlayers = dbPlayers
                .AsParallel()
                .Where(p =>
                {
                    SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

                    return user is not null && user.IsAPlayer() && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel;
                })
                .ToArray();

            if (!availablePlayers.Any())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.WarAddNoPlayers(dbGuild.MinimalTownHallLevel));

                return;
            }

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.WAR_ADD_SELECT_HOUR);

            Enumerable.Range(0, 24)
                .Reverse()
                .ToList()
                .ForEach(n => menuBuilder.AddOption(warDate.AddHours(n).ToString("t", cultureInfo), n.ToString()));

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = commandText.WarAddChooseHour(cOpponent.Name, warDate.ToString("D", cultureInfo));
                msg.Components = new(componentBuilder.Build());
            });

            // Registers informations into storage
            ComponentStorage storage = ComponentStorage.GetInstance();

            string[] datas = new[] { opponentTag, totalTime.ToString(), date };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



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
