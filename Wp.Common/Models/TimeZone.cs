using System.Reflection;
using Wp.Database.Services.Attributes;

namespace Wp.Database.Models
{
    public enum TimeZone
    {
        /// <summary>
        /// Hawaii Standard Time (-10:00) and Hawaii Daylight Time (-09:00) timezones
        /// </summary>
        [TimeZone("HST / HDT", "HST")]
        HST_HDT = 0,

        /// <summary>
        /// Pacific Standard Time (-08:00) and Pacific Daylight Time (-07:00) timezones
        /// </summary>
        [TimeZone("PST / PDT", "PST8PDT")]
        PST_PDT = 1,

        /// <summary>
        /// Moutain Standard Time (-07:00) and Moutain Daylight Time (-06:00) timezones
        /// </summary>
        [TimeZone("MST / MDT", "MST7MDT")]
        MST_MDT = 2,

        /// <summary>
        /// Central Standard Time (-06:00) and Central Daylight Time (-05:00) timezones
        /// </summary>
        [TimeZone("CST / CDT", "CST6CDT")]
        CST_CDT = 3,

        /// <summary>
        /// Eastern Standard Time (-05:00) and Eastern Daylight Time (-04:00) timezones
        /// </summary>
        [TimeZone("EST / EDT", "EST5EDT")]
        EST_EDT = 4,

        /// <summary>
        /// Greenwich Mean Time (00:00) timezone
        /// </summary>
        [TimeZone("GMT", "Etc/GMT")]
        GMT = 5,

        /// <summary>
        /// Central European Time (+01:00) and Central European Summer Time (+02:00) timezones
        /// </summary>
        [TimeZone("CET / CEST", "CET")]
        CET_CEST = 6,

        /// <summary>
        /// Moscow Standard Time (+03:00) timezone
        /// </summary>
        [TimeZone("MSK", "Europe/Moscow")]
        MSK = 7,

        /// <summary>
        /// Indian Standard Time (+05:30) timezone
        /// </summary>
        [TimeZone("IST", "Asia/Kolkata")]
        IST = 8,
    }

    public static class TimeZoneExtensions
    {
        /// <summary>
        /// Converts the value of this instance to its equivalent TimeZoneAttribute representation
        /// </summary>
        /// <returns>The TimeZoneAttribute representation of the value of this instance</returns>
        public static TimeZoneAttribute AsAttribute(this TimeZone timeZone)
        {
            return timeZone
                .GetType()?
                .GetMember(timeZone.ToString())?
                .First()?
                .GetCustomAttribute<TimeZoneAttribute>() ?? throw new ArgumentNullException(nameof(timeZone));
        }
    }
}
