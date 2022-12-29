using TimeZone = Wp.Common.Models.TimeZone;

namespace Wp.Common.Settings
{
	public static class DefaultParameters
	{
		public static readonly string DEFAULT_TIME_ADDITIONAL = "-";

		public static readonly int DEFAULT_NUMBER_CALENDAR_DISPLAY_PER_DAY = 1;
		public static readonly int DEFAULT_NUMBER_REMIND = 1;

		public static readonly TimeZone DEFAULT_TIME_ZONE = TimeZone.CET_CEST;
	}
}
