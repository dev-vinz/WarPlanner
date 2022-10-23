using TimeZone = Wp.Common.Models.TimeZone;

namespace Wp.Common.Services.NodaTime
{
	public interface INodaConverter
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		DateTimeOffset ConvertDateTo(DateTimeOffset date, TimeZone timeZone);
		DateTimeOffset ConvertNowTo(TimeZone timeZone);

		DateTimeOffset ConvertTime(int hour, int minute, int second, TimeZone source, TimeZone destination);
		DateTimeOffset ConvertTime(int hour, int minute, TimeZone source, TimeZone destination);

		DateTimeOffset ConvertTimeFrom(int hour, int minute, int second, TimeZone source);
		DateTimeOffset ConvertTimeFrom(int hour, int minute, TimeZone source);
	}
}
