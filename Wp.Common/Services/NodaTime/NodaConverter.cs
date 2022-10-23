using NodaTime;
using Wp.Common.Models;

namespace Wp.Common.Services.NodaTime
{
	public class NodaConverter : INodaConverter
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public DateTimeOffset ConvertDateTo(DateTimeOffset date, Models.TimeZone timeZone)
		{
			Instant instant = Instant.FromDateTimeOffset(date.UtcDateTime);
			ZonedDateTime zoned = instant.InZone(timeZone.AsAttribute().Zone);

			return new DateTimeOffset(zoned.Year, zoned.Month, zoned.Day, zoned.Hour, zoned.Minute, zoned.Second, zoned.Offset.ToTimeSpan());
		}

		public DateTimeOffset ConvertNowTo(Models.TimeZone timeZone)
		{
			return ConvertDateTo(DateTimeOffset.UtcNow, timeZone);
		}

		public DateTimeOffset ConvertTime(int hour, int minute, int second, Models.TimeZone source, Models.TimeZone destination)
		{
			DateTimeOffset sourceDate = ConvertTimeFrom(hour, minute, second, source);

			return ConvertDateTo(sourceDate, destination);
		}

		public DateTimeOffset ConvertTime(int hour, int minute, Models.TimeZone source, Models.TimeZone destination)
		{
			return ConvertTime(hour, minute, 0, source, destination);
		}

		public DateTimeOffset ConvertTimeFrom(int hour, int minute, int second, Models.TimeZone source)
		{
			DateTimeOffset sourceNow = ConvertNowTo(source);

			DateTimeOffset sourceTime = new DateTimeOffset(sourceNow.Date, sourceNow.Offset)
				.AddHours(hour)
				.AddMinutes(minute)
				.AddSeconds(second);

			return sourceTime;
		}

		public DateTimeOffset ConvertTimeFrom(int hour, int minute, Models.TimeZone source)
		{
			return ConvertTimeFrom(hour, minute, 0, source);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
	}
}
