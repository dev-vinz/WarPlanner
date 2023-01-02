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

		public string WarRemindApiOffline(string player, string competition, string remaining);
		public string WarRemindWarnPlayer(string player, string clan, string competition, string opponent, string start, string remaining);
	}
}
