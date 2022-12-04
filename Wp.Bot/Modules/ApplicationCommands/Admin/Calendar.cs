using Discord;
using Discord.Interactions;
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
            await RespondAsync(channel.ToString(), ephemeral: true);
        }

        [SlashCommand("options", "Change some options of the calendar", runMode: RunMode.Async)]
        public async Task Options()
        {
            await RespondAsync("Sous forme de choix de bouton comme `/competition edit` : stop, remind_war, choose_remind & choose_display", ephemeral: true);
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
