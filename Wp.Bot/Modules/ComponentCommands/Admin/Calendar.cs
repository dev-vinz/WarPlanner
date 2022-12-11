using Discord.Interactions;
using Discord.WebSocket;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database.Services;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands.Admin
{
    public class Calendar : InteractionModuleBase<SocketInteractionContext>
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

        public Calendar(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          BUTTON COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_CHOOSE_REMIND_WAR, runMode: RunMode.Async)]
        public async Task ChooseRemind()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Choose remind", ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY, runMode: RunMode.Async)]
        public async Task Display()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            DbTimes times = Database.Context.Times;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Common.Models.Calendar? dbCalendar = calendars
                .AsParallel()
                .FirstOrDefault(c => c.Guild == dbGuild);

            Time[] dbTimes = times
                .AsParallel()
                .Where(t => t.Guild == dbGuild)
                .ToArray();

            Time? displayTime = dbTimes.FirstOrDefault(t => t.Action == TimeAction.DISPLAY_CALENDAR);

            // Gets interaction responses
            IAdmin interactionText = dbGuild.AdminText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            if (dbCalendar is null)
            {
                await FollowupAsync(interactionText.CalendarIdNotSet, ephemeral: true);

                return;
            }

            if (dbCalendar.ChannelId is null)
            {
                await FollowupAsync(interactionText.CalendarOptionChanneNotSet, ephemeral: true);

                return;
            }

            if (displayTime is not null)
            {
                // Disable

                times.Remove(displayTime);

                await FollowupAsync(interactionText.CalendarOptionsDisplayDisabled, ephemeral: true);
            }
            else
            {
                // Enable

                SocketTextChannel channel = Context.Guild.GetTextChannel((ulong)dbCalendar.ChannelId);

                displayTime = new(dbGuild, TimeAction.DISPLAY_CALENDAR, DateTimeOffset.UtcNow, Time.CALENDAR_INTERVAL, DefaultParameters.DEFAULT_TIME_ADDITIONAL)
                {
                    // Number of display per day
                    Optional = DefaultParameters.NUMBER_CALENDAR_DISPLAY_PER_DAY.ToString(),
                };

                times.Add(displayTime);

                await FollowupAsync(interactionText.CalendarOptionsDisplayEnabled(channel.Mention), ephemeral: true);
            }
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY_FREQUENCY, runMode: RunMode.Async)]
        public async Task DisplayFrequency()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Display frequency", ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_REMIND_WAR, runMode: RunMode.Async)]
        public async Task RemindWar()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Enable / Disable remind war", ephemeral: true);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    }
}
