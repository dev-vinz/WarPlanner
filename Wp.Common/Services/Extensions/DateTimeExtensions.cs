namespace Wp.Common.Services.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Removes the seconds of a DateTime, and returns a copy of it
		/// </summary>
		/// <param name="dateTime">The DateTime to remove seconds</param>
		/// <returns>The truncated copy of a DateTime</returns>
		public static DateTime TruncSeconds(this DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
		}
	}
}
