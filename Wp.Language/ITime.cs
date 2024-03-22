namespace Wp.Language
{
    public interface ITime
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       CALENDAR DISPLAY EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarDisplayCannotSend { get; }
        public string CalendarDisplayMissingAccess { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          GUILD JOIN EVENT                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string GuildJoinedTitle { get; }
        public string GuildJoinedDescription(string username);
        public string GuildJoinedFieldTimeZoneTitle { get; }
        public string GuildJoinedFieldTimeZoneDescription(string timeZone, double offset);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          WAR REMIND EVENT                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarRemindManagerApiOffline(string competition, string remaining);
        public string WarRemindPlayerApiOffline(string player, string competition, string remaining);
        public string WarRemindWarnManager(string players, string clan, string competition, string remaining);
        public string WarRemindWarnPlayer(string player, string clan, string competition, string opponent, string start, string remaining);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      WAR REMIND STATUS EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarRemindStatusManagerApiOffline(string competition);
        public string WarRemindStatusPlayerApiOffline(string player, string competition);
        public string WarRemindStatusWarnManager(string players, string competition);
        public string WarRemindStatusWarnPlayer(string player, string competition);
    }
}
