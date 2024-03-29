﻿using Discord;
using Discord.WebSocket;
using Wp.Bot.Modules.TimeEvents;
using Wp.Bot.Services.Logger.Event;
using Wp.Common.Models;
using Wp.Common.Services;
using Wp.Common.Settings;
using Wp.Database.Services;

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

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



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

            return true;
        }

        private Task HandleTime()
        {
            // Have to be in an other thread
            new Thread(async () =>
            {
                // Until database is verified and operational
                while (!isDatabaseVerified)
                {
                    // Wait
                    Thread.Sleep(10);
                }

                await logger.ClientReadyAsync();

                try
                {
                    JSFunctions.SetInterval(() =>
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
                            }, TimeSpan.FromSeconds(15));
                }
                catch (Exception e)
                {
                    LogMessage msg = new(LogSeverity.Critical, "Time Loop", "Error...", e);
                    Console.WriteLine(msg);
                    throw;
                }

                LogMessage logMessage = new(LogSeverity.Info, "Discord", "Time loop started");
                Console.WriteLine(logMessage.ToString());
            }).Start();

            return Task.CompletedTask;
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
