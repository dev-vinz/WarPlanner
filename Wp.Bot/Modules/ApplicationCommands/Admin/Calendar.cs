using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Wp.Api;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database.Services;
using Wp.Language;
using GoogleCalendar = Google.Apis.Calendar.v3.Data.Calendar;

namespace Wp.Bot.Modules.ApplicationCommands.Admin
{
    [Group("calendar", "Calendar commands handler")]
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
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [SlashCommand("channel", "Set or replace the channel where the calendar is displayed", runMode: RunMode.Async)]
        public async Task Channel([Summary("channel", "The channel where the calendar is displayed")] SocketChannel channel)
        {
            await DeferAsync(true);

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Gets command responses
            IAdmin commandText = dbGuild.AdminText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            if (channel is not SocketTextChannel guildChannel)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ChannelNotText);

                return;
            }

            // Filters for guild
            Common.Models.Calendar? dbCalendar = calendars
                .AsParallel()
                .Where(c => c.Guild == dbGuild)
                .FirstOrDefault();

            if (dbCalendar is null)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.CalendarIdNotSet);

                return;
            }

            RestUserMessage? restUserMessage = null;

            try
            {
                restUserMessage = await guildChannel.SendMessageAsync(commandText.CalendarWillBeDisplayedHere(Context.User.Mention));
            }
            catch (Exception)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.NotThePermissionToWrite);

                return;
            }

            bool isAdd = dbCalendar.ChannelId is null;

            // Change dbCalendar values
            dbCalendar.ChannelId = guildChannel.Id;
            dbCalendar.MessageId = restUserMessage.Id;

            // Update object
            calendars.Update(dbCalendar);

            if (isAdd)
            {
                // Add time object corresponding to calendar announcement

                Time displayTime = new(dbGuild, TimeAction.DISPLAY_CALENDAR, DateTimeOffset.UtcNow, Time.CALENDAR_INTERVAL, DefaultParameters.DEFAULT_TIME_ADDITIONAL)
                {
                    // Number of display per day
                    Optional = DefaultParameters.DEFAULT_NUMBER_CALENDAR_DISPLAY_PER_DAY.ToString(),
                };

                Database.Context.Times.Add(displayTime);
            }

            await ModifyOriginalResponseAsync(msg => msg.Content = commandText.CalendarChannelChanged(guildChannel.Mention));
        }

        [SlashCommand("options", "Change some options of the calendar", runMode: RunMode.Async)]
        public async Task Options()
        {
            await DeferAsync(true);

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            DbTimes times = Database.Context.Times;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Common.Models.Calendar? dbCalendar = calendars
                .AsParallel()
                .Where(c => c.Guild == dbGuild)
                .FirstOrDefault();

            Time[] dbTimes = times
                .AsParallel()
                .Where(t => t.Guild == dbGuild)
                .ToArray();

            // Gets command responses
            IAdmin commandText = dbGuild.AdminText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            if (dbCalendar is null)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.CalendarIdNotSet);

                return;
            }

            PremiumLevel premiumRequired = PremiumLevel.LOW;

            // (Dis)activate calendar display button
            Time? display = dbTimes.FirstOrDefault(t => t.Action == TimeAction.DISPLAY_CALENDAR);
            ButtonBuilder displayButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.CalendarOptionsDisplayed(display is not null))
                .WithStyle(display is not null ? ButtonStyle.Danger : ButtonStyle.Success)
                .WithCustomId(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY);

            // Choose calendar display frequency button
            ButtonBuilder frequencyButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.CalendarOptionsFrequency(display?.Optional))
                .WithStyle(ButtonStyle.Secondary)
                .WithDisabled(dbGuild.PremiumLevel < premiumRequired || display is null)
                .WithCustomId(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY_FREQUENCY);

            // (Dis)activate remind war button
            Time? remind = dbTimes.FirstOrDefault(t => t.Action == TimeAction.REMIND_WAR);
            ButtonBuilder remindButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.CalendarOptionsRemind(remind is not null))
                .WithStyle(remind is not null ? ButtonStyle.Danger : ButtonStyle.Success)
                .WithCustomId(IdProvider.CALENDAR_OPTIONS_BUTTON_REMIND_WAR);

            // Choose reminds before war button
            ButtonBuilder nbRemindsButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.CalendarOptionsChooseRemind(remind?.Optional))
                .WithStyle(ButtonStyle.Secondary)
                .WithDisabled(dbGuild.PremiumLevel < premiumRequired || remind is null)
                .WithCustomId(IdProvider.CALENDAR_OPTIONS_BUTTON_CHOOSE_REMIND_WAR);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithButton(displayButtonBuilder, 0)
                .WithButton(frequencyButtonBuilder, 0)
                .WithButton(remindButtonBuilder, 1)
                .WithButton(nbRemindsButtonBuilder, 1);

            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = commandText.CalendarChooseOption;
                msg.Components = new(componentBuilder.Build());
            });
        }

        [SlashCommand("set", "Set or replace the calendar ID", runMode: RunMode.Async)]
        public async Task Set([Summary("id", "The calendar ID")] string id)
        {
            await DeferAsync(true);

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Common.Models.Calendar? dbCalendar = calendars
                .AsParallel()
                .Where(c => c.Guild == dbGuild)
                .FirstOrDefault();

            // Gets command responses
            IAdmin commandText = dbGuild.AdminText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            GoogleCalendar? calendar = await GoogleCalendarApi.Calendars.GetAsync(id);

            if (calendar is null)
            {
                // Documentation button
                ButtonBuilder docButtonBuilder = new ButtonBuilder()
                    .WithLabel(generalResponses.Documentation)
                    .WithStyle(ButtonStyle.Link)
                    .WithUrl(Utilities.GITBOOK_DOCUMENTATION);

                // Build component
                ComponentBuilder componentBuilder = new ComponentBuilder()
                    .WithButton(docButtonBuilder);

                await ModifyOriginalResponseAsync(msg =>
                {
                    msg.Content = generalResponses.GoogleCannotAccessCalendar;
                    msg.Components = new(componentBuilder.Build());
                });

                return;
            }

            bool isAdd = dbCalendar is null;

            if (isAdd)
            {
                // Add calendar
                calendars.Add(new(dbGuild, id));
            }
            else
            {
                // Update calendar
                dbCalendar!.Id = id;
                calendars.Update(dbCalendar);
            }

            await ModifyOriginalResponseAsync(msg => msg.Content = isAdd ? commandText.CalendarIdAdded(id) : commandText.CalendarIdUpdated(id));
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
