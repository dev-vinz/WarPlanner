using Discord;
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

            Time? remindTime = dbTimes.FirstOrDefault(t => t.Action == TimeAction.REMIND_WAR);

            // Gets interaction responses
            IAdmin interactionText = dbGuild.AdminText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            if (dbCalendar is null)
            {
                await FollowupAsync(interactionText.CalendarIdNotSet, ephemeral: true);

                return;
            }

            if (remindTime is null)
            {
                await FollowupAsync(interactionText.CalendarOptionRemindNotEnabled, ephemeral: true);

                return;
            }

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithMinValues(1)
                .WithMaxValues(Settings.CALENDAR_REMIND_WAR.Count)
                .WithCustomId(IdProvider.CALENDAR_OPTIONS_REMIND_WAR_SELECT_NUMBER_REMINDS);

            Settings.CALENDAR_REMIND_WAR
                .ToList()
                .ForEach(kvp => menuBuilder.AddOption(interactionText.CalendarOptionRemindFrequencyLabel(kvp.Value), kvp.Key.ToString()));

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            await FollowupAsync(interactionText.CalendarOptionRemindFrequencyChoose, components: componentBuilder.Build(), ephemeral: true);
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
                await FollowupAsync(interactionText.CalendarOptionChannelNotSet, ephemeral: true);

                return;
            }

            if (displayTime is not null)
            {
                // Disable

                times.Remove(displayTime);

                await FollowupAsync(interactionText.CalendarOptionDisplayDisabled, ephemeral: true);
            }
            else
            {
                // Enable

                SocketTextChannel channel = Context.Guild.GetTextChannel((ulong)dbCalendar.ChannelId);

                displayTime = new(dbGuild, TimeAction.DISPLAY_CALENDAR, DateTimeOffset.UtcNow, Time.CALENDAR_INTERVAL, DefaultParameters.DEFAULT_TIME_ADDITIONAL)
                {
                    // Number of display per day
                    Optional = DefaultParameters.DEFAULT_NUMBER_CALENDAR_DISPLAY_PER_DAY.ToString(),
                };

                times.Add(displayTime);

                await FollowupAsync(interactionText.CalendarOptionDisplayEnabled(channel.Mention), ephemeral: true);
            }
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY_FREQUENCY, runMode: RunMode.Async)]
        public async Task DisplayFrequency()
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
                await FollowupAsync(interactionText.CalendarOptionChannelNotSet, ephemeral: true);

                return;
            }

            if (displayTime is null)
            {
                await FollowupAsync(interactionText.CalendarOptionDisplayNotEnabled, ephemeral: true);

                return;
            }

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.CALENDAR_OPTIONS_DISPLAY_SELECT_DISPLAY_FREQUENCY);

            Enumerable.Range(1, Settings.MAX_CALENDAR_DISPLAY_PER_DAY)
                .Reverse()
                .ToList()
                .ForEach(i => menuBuilder.AddOption(interactionText.CalendarOptionDisplayFrequencyPerDayLabel(i), i.ToString(), interactionText.CalendarOptionDisplayFrequencyPerDayDescription(i)));

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            await FollowupAsync(interactionText.CalendarOptionDisplayFrequencyChoose, components: componentBuilder.Build(), ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_REMIND_WAR, runMode: RunMode.Async)]
        public async Task RemindWar()
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

            Time? remindTime = dbTimes.FirstOrDefault(t => t.Action == TimeAction.REMIND_WAR);

            // Gets interaction responses
            IAdmin interactionText = dbGuild.AdminText;

            if (dbCalendar is null)
            {
                await FollowupAsync(interactionText.CalendarIdNotSet, ephemeral: true);

                return;
            }

            if (remindTime is not null)
            {
                // Disable

                times.Remove(remindTime);

                await FollowupAsync(interactionText.CalendarOptionRemindDisabled, ephemeral: true);
            }
            else
            {
                // Enable

                remindTime = new(dbGuild, TimeAction.REMIND_WAR, DateTimeOffset.UtcNow, Time.CALENDAR_INTERVAL, DefaultParameters.DEFAULT_TIME_ADDITIONAL)
                {
                    // Number of display per day
                    Optional = DefaultParameters.DEFAULT_NUMBER_REMIND.ToString(),
                };

                times.Add(remindTime);

                await FollowupAsync(interactionText.CalendarOptionRemindEnabled, ephemeral: true);
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_DISPLAY_SELECT_DISPLAY_FREQUENCY, runMode: RunMode.Async)]
        public async Task ChooseFrequency(string[] selections)
        {
            string option = selections.First();

            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Loads databases infos
            DbTimes times = Database.Context.Times;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Time[] dbTimes = times
                .AsParallel()
                .Where(t => t.Guild == dbGuild)
                .ToArray();

            Time displayTime = dbTimes.First(t => t.Action == TimeAction.DISPLAY_CALENDAR);

            // Update and save
            displayTime.Optional = option;
            times.Update(displayTime);

            // Gets interaction responses
            IAdmin interactionText = dbGuild.AdminText;

            await FollowupAsync(interactionText.CalendarOptionDisplayFrequencyUpdated(option), ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_REMIND_WAR_SELECT_NUMBER_REMINDS, runMode: RunMode.Async)]
        public async Task ChooseRemind(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Transforms into options
            int[] options = selections
                .Select(s => int.Parse(s))
                .OrderBy(o => o)
                .ToArray();

            // Loads databases infos
            DbTimes times = Database.Context.Times;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Time[] dbTimes = times
                .AsParallel()
                .Where(t => t.Guild == dbGuild)
                .ToArray();

            Time remindTime = dbTimes.First(t => t.Action == TimeAction.REMIND_WAR);

            // Update and save
            remindTime.Optional = string.Join("", options);
            times.Update(remindTime);

            // Gets interaction responses
            IAdmin interactionText = dbGuild.AdminText;

            await FollowupAsync(interactionText.CalendarOptionRemindFrequencyUpdated(options.Select(o => Settings.CALENDAR_REMIND_WAR.GetValueOrDefault(o)).ToArray()), ephemeral: true);
        }
    }
}
