using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Wp.Api.Models;
using Wp.Api.Settings;

namespace Wp.Api
{
    public static class GoogleCalendarApi
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly CalendarService service;
        private static readonly string email = "warplanner@vinz-discord.iam.gserviceaccount.com";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        static GoogleCalendarApi()
        {
            ServiceAccountCredential credential = new(
                new ServiceAccountCredential.Initializer(email)
                {
                    Scopes = new string[] { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents }
                }.FromPrivateKey(Keys.GOOGLE_CALENDAR_TOKEN)
            );

            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CALENDAR METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Accesses to calendars methods
        /// </summary>
        public static class Calendars
        {
            /// <summary>
            /// Gets a Google Calendar by its id
            /// </summary>
            /// <param name="calendarId">A Google Calendar id</param>
            /// <returns>A Google Calendar</returns>
            public static async Task<Calendar?> GetAsync(string calendarId)
            {
                Calendar? calendar = null;

                try
                {
                    calendar = await service.Calendars.Get(calendarId).ExecuteAsync();
                }
                catch (Exception)
                {
                }

                return calendar;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           EVENTS METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Accesses to events methods
        /// </summary>
        public static class Events
        {
            public static async Task<Event?> InsertAsync(CalendarEvent calendarEvent, string zoneId, string calendarId)
            {
                Event? result = null;
                Event matchEvent = new()
                {
                    Summary = calendarEvent.CompetitionName,
                    Location = calendarEvent.OpponentTag,
                    Start = new EventDateTime
                    {
                        DateTime = calendarEvent.Start.UtcDateTime,
                        TimeZone = zoneId,
                    },
                    End = new EventDateTime
                    {
                        DateTime = calendarEvent.End.UtcDateTime,
                        TimeZone = zoneId,
                    },
                    Description = string.Join("\n", calendarEvent.Players),
                };

                try
                {
                    result = await service.Events.Insert(matchEvent, calendarId).ExecuteAsync();
                }
                catch (Exception)
                {
                }

                return result;
            }
        }
    }
}
