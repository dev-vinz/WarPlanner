using System.Globalization;
using Wp.Common.Services.NodaTime;
using Wp.Language;
using Wp.Language.English;
using Wp.Language.French;

namespace Wp.Common.Models
{
	public class Guild
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly ulong id;
		private Language language;
		private TimeZone timeZone;
		private PremiumLevel premiumLevel;
		private uint minimalTownHallLevel;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Gets the discord server's id
		/// </summary>
		public ulong Id { get => id; }

		/// <summary>
		/// Gets / Sets the bot's language
		/// </summary>
		public Language Language { get => language; set => language = value; }

		/// <summary>
		/// Gets / Sets the discord server's TimeZone
		/// </summary>
		public TimeZone TimeZone { get => timeZone; set => timeZone = value; }

		/// <summary>
		/// Gets / Sets the discord server's premium level
		/// </summary>
		public PremiumLevel PremiumLevel { get => premiumLevel; set => premiumLevel = value; }

		/// <summary>
		/// Gets / Sets the minimal town hall level "filter" for the list during a war adding command
		/// </summary>
		public uint MinimalTownHallLevel { get => minimalTownHallLevel; set => minimalTownHallLevel = value; }

		/* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Gets the culture info of the guild
		/// </summary>
		public CultureInfo CultureInfo => language.GetCultureInfo();

		/// <summary>
		/// Gets the current time and date object with the offset of the server's TimeZone
		/// </summary>
		public DateTimeOffset Now => new NodaConverter().ConvertNowTo(timeZone);

		/// <summary>
		/// Gets the admin commands texts, depending on the guild's language
		/// </summary>
		public IAdmin AdminText => language switch
		{
			Language.ENGLISH => new AdminEnglish(),
			Language.FRENCH => new AdminFrench(),
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
		};

		/// <summary>
		/// Gets the general responses text, depending on the guild's language
		/// </summary>
		public IGeneralResponse GeneralResponses => language switch
		{
			Language.ENGLISH => new EnglishGeneralResponse(),
			Language.FRENCH => new FrenchGeneralResponse(),
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
		};

		/// <summary>
		/// Gets the global commands texts, depending on the guild's language
		/// </summary>
		public IGlobal GlobalText => language switch
		{
			Language.ENGLISH => new GlobalEnglish(),
			Language.FRENCH => new GlobalFrench(),
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
		};

		/// <summary>
		/// Gets the manager commands texts, depending on the guild's language
		/// </summary>
		public IManager ManagerText => language switch
		{
			Language.ENGLISH => new ManagerEnglish(),
			Language.FRENCH => new ManagerFrench(),
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
		};

		/// <summary>
		/// Gets the time events texts, depending on the guild's language
		/// </summary>
		public ITime TimeText => language switch
		{
			Language.ENGLISH => new TimeEnglish(),
			Language.FRENCH => new TimeFrench(),
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
		};

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Represents a discord server
		/// </summary>
		/// <param name="id">A discord server's id</param>
		/// <param name="timeZone">A TimeZone used by the discord server</param>
		public Guild(ulong id, TimeZone timeZone)
		{
			// Inputs
			{
				this.id = id;
				this.timeZone = timeZone;
			}

			// Defaults
			{
				language = Language.FRENCH;
				premiumLevel = PremiumLevel.HIGH;
				minimalTownHallLevel = 0;
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public override bool Equals(object? obj)
		{
			// Check for null and compare run-time types.
			if ((obj == null) || !GetType().Equals(obj.GetType()))
			{
				return false;
			}
			else
			{
				Guild? guild = obj as Guild;

				return Id == guild?.Id;
			}
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static bool operator ==(Guild? x, Guild? y)
		{
			if (x is null && y is null)
			{
				return true;
			}
			else
			{
				return x?.Equals(y) ?? false;
			}
		}

		public static bool operator !=(Guild? x, Guild? y)
		{
			return !(x == y);
		}
	}
}
