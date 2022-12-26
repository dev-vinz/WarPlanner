namespace Wp.Common.Settings
{
	public static class Settings
	{
		public static readonly int MAX_CALENDAR_DISPLAY_PER_DAY = 4;
		public static readonly int MAX_OPTION_PER_SELECT_MENU = 20;
		public static readonly int NB_WAR_PROPOSED_DATES = 14;

		public static readonly IReadOnlyDictionary<int, int> CALENDAR_REMIND_WAR = new Dictionary<int, int>()
		{
			{ 1, 120 }, // 2h
			{ 2, 060 }, // 1h
			{ 3, 030 }, // 30min
			{ 4, 015 }, // 15min
			{ 5, 010 }, // 10min
			{ 6, 005 }, // 5min
			{ 7, 003 }, // 3min
		};
	}
}
