namespace Wp.Language
{
    public interface ITime
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       CALENDAR DISPLAY EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarDisplayCannotSend { get; }

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
