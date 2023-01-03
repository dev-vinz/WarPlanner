namespace Wp.Language.English
{
	public class TimeEnglish : ITime
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       CALENDAR DISPLAY EVENT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarDisplayCannotSend => "The channel where the clanedar was displayed has been removed";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          WAR REMIND EVENT                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarRemindApiOffline(string player, string competition, string remaining) => $"Hello {player} !" +
			$"\nI currently can't verify if you're in the clan, but in doubt, don't forget your match in **{competition}**" +
			$"\nIt starts in {remaining}";

		public string WarRemindWarnPlayer(string player, string clan, string competition, string opponent, string start, string remaining) => $"Hello {player}, don't forget to join **{clan}** for your match in `{competition}` against **{opponent}** !" +
			$"\nIt starts at **{start}**, only {remaining} remaining";
	}
}
