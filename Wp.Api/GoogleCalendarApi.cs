using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
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
            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(email)
                {
                    Scopes = new string[] { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents }
                }.FromPrivateKey(Keys.GoogleCalendarToken)
            );

            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CALENDAR METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static class Calendars
        {
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

        public static class Events
        {

        }
    }
}
