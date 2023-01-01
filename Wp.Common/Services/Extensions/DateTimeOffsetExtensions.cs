namespace Wp.Common.Services.Extensions
{
	public static class DateTimeOffsetExtensions
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          BOOLEAN METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Indicates wether a date is between two dates, at the second
		/// </summary>
		/// <param name="source">The source date</param>
		/// <param name="start">The first date to compare</param>
		/// <param name="end">The second date to compare</param>
		/// <returns>A flag that indicates wether a date is between two other dates, at the second</returns>
		public static bool IsBetweenSeconds(this DateTimeOffset source, DateTimeOffset start, DateTimeOffset end)
		{
			source = source.ToUniversalTime();
			start = start.ToUniversalTime();
			end = end.ToUniversalTime();

			return start.Year <= source.Year && source.Year <= end.Year &&
					start.Month <= source.Month && source.Month <= end.Month &&
					start.Day <= source.Day && source.Day <= end.Day &&
					start.Hour <= source.Hour && source.Hour <= end.Hour &&
					start.Minute <= source.Minute && source.Minute <= end.Minute &&
					start.Second <= source.Second && source.Second < end.Second;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      DATE TIME OFFSET METHODS                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Removes the seconds of a DateTime, and returns a copy of it
		/// </summary>
		/// <param name="dateTime">The DateTime to remove seconds</param>
		/// <returns>The truncated copy of a DateTime</returns>
		public static DateTimeOffset TruncSeconds(this DateTimeOffset dateTime)
		{
			return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Offset);
		}
	}
}
