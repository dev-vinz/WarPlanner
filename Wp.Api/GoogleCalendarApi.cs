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
            /// <summary>
            /// Deletes an event from a Google Calendar
            /// </summary>
            /// <param name="calendarId">A Google Calendar id</param>
            /// <param name="eventId">A Google Calendar event id</param>
            /// <returns>A confirmation that the event has been deleted. true if it is; false otherwise</returns>
            public static async Task<bool> DeleteAsync(string calendarId, string eventId)
            {
                string? result = null;

                try
                {
                    result = await service.Events.Delete(calendarId, eventId).ExecuteAsync();
                }
                catch (Exception)
                {
                }

                return result is not null;
            }

            /// <summary>
            /// Gets a calendar event from a Google Calendar
            /// </summary>
            /// <param name="calendarId">A Google Calendar id</param>
            /// <param name="eventId">A Google Calendar event id</param>
            /// <returns>A calendar event from a Google Calendar</returns>
            public static async Task<CalendarEvent?> GetAsync(string calendarId, string eventId)
            {
                CalendarEvent? calendarEvent = null;

                try
                {
                    Event? result = await service.Events.Get(calendarId, eventId).ExecuteAsync();

                    if (result != null)
                    {
                        string[] playersTag = result.Description?
                            .Split("\n", StringSplitOptions.RemoveEmptyEntries)?
                            .ToArray() ?? Array.Empty<string>();

                        calendarEvent = new(result.Id, result.Summary, result.Location, DateTimeOffset.Parse(result.Start.DateTimeRaw), DateTimeOffset.Parse(result.End.DateTimeRaw), playersTag);
                    }
                }
                catch (Exception)
                {
                }

                return calendarEvent;
            }

            /// <summary>
            /// Inserts a new calendar event into a Google Calendar
            /// </summary>
            /// <param name="calendarEvent">A calendar event</param>
            /// <param name="zoneId">A TimeZone id</param>
            /// <param name="calendarId">A calendar id, where the event has to be inserted</param>
            /// <returns>A confirmation of the successful insertion of event</returns>
            public static async Task<Event?> InsertAsync(CalendarEvent calendarEvent, string zoneId, string calendarId)
            {
                Event? result = null;
                Event matchEvent = new()
                {
                    Summary = calendarEvent.CompetitionName,
                    Location = calendarEvent.OpponentTag,
                    Start = new EventDateTime
                    {
                        DateTimeDateTimeOffset = calendarEvent.Start.UtcDateTime,
                        TimeZone = zoneId,
                    },
                    End = new EventDateTime
                    {
                        DateTimeDateTimeOffset = calendarEvent.End.UtcDateTime,
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

            /// <summary>
            /// Gets all the calendar events from a Google Calendar
            /// </summary>
            /// <param name="calendarId">A Google Calendar id</param>
            /// <param name="fromNow">A flag that indicates if we have to take only next events from now</param>
            /// <returns>An array containing all the calendar events from a Google Calendar</returns>
            public static async Task<CalendarEvent[]> ListAsync(string calendarId, bool fromNow = true)
            {
                List<CalendarEvent> list = new();

                // If fromNow is false, take only 1 month before
                DateTimeOffset dateUtc = !fromNow ? DateTimeOffset.UtcNow.AddMonths(-1) : DateTimeOffset.UtcNow;

                try
                {
                    Google.Apis.Calendar.v3.Data.Events? result = await service.Events.List(calendarId).ExecuteAsync();
                    Event[]? items = result.Items.Where(i => i.Start?.DateTimeRaw != null && DateTimeOffset.Parse(i.Start.DateTimeRaw).UtcDateTime > dateUtc).ToArray();

                    foreach (Event @event in items)
                    {
                        DateTimeOffset date = DateTimeOffset.Parse(@event.Start.DateTimeRaw);

                        CalendarEvent? calendarEvent = await GetAsync(calendarId, @event.Id);

                        if (calendarEvent != null)
                            list.Add(calendarEvent);
                    }
                }
                catch (Exception)
                {
                }

                return list.OrderBy(e => e.Start).ToArray();
            }

            /// <summary>
            /// Updates a calendar event in a Google Calendar
            /// </summary>
            /// <param name="calendarEvent">A calendar event updated</param>
            /// <param name="calendarId">A Google Calendar id</param>
            /// <returns>A boolean that indicates wether the event has been updated or not</returns>
            public static async Task<bool> UpdateAsync(CalendarEvent calendarEvent, string calendarId)
            {
                try
                {
                    Event? oldEvent = await service.Events.Get(calendarId, calendarEvent.Id).ExecuteAsync();

                    if (oldEvent == null) return false;

                    Event newEvent = new()
                    {
                        Summary = calendarEvent.CompetitionName,
                        Location = calendarEvent.OpponentTag,
                        Start = new EventDateTime
                        {
                            DateTimeDateTimeOffset = calendarEvent.Start.UtcDateTime,
                            TimeZone = oldEvent.Start.TimeZone,
                        },
                        End = new EventDateTime
                        {
                            DateTimeDateTimeOffset = calendarEvent.End.UtcDateTime,
                            TimeZone = oldEvent.End.TimeZone,
                        },
                        Description = string.Join("\n", calendarEvent.Players),
                    };

                    await service.Events.Update(newEvent, calendarId, calendarEvent.Id).ExecuteAsync();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
