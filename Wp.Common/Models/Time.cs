namespace Wp.Database.Models
{
    public class Time
    {
        public static readonly int CALENDAR_INTERVAL = 60;
        public static readonly int REMIND_INTERVAL = 60;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly Guild guild;
        private readonly TimeAction action;
        private DateTimeOffset date;
        private readonly int interval;
        private string additional;
        private string? optional;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the discord server associated at this instance
        /// </summary>
        public Guild Guild { get => guild; }

        /// <summary>
        /// Gets the purpose of this instance
        /// </summary>
        public TimeAction Action { get => action; }

        /// <summary>
        /// Gets / Sets the last date this instance has been checked
        /// </summary>
        public DateTimeOffset Date { get => date; set => date = value; }

        /// <summary>
        /// Gets the interval between to scans
        /// </summary>
        public int Interval { get => interval; }

        /// <summary>
        /// Gets / Sets an additional information
        /// </summary>
        public string Additional { get => additional; set => additional = value; }

        /// <summary>
        /// Gets / Sets and optional information
        /// </summary>
        public string? Optional { get => optional; set => optional = value; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents a time activity inside a discord server
        /// </summary>
        /// <param name="guild">A discord server, represented by a Guild object</param>
        /// <param name="action">An action that defines the instance</param>
        /// <param name="date">The last time this has been checked</param>
        /// <param name="interval">An interval between 2 scans</param>
        /// <param name="additional">An additional information, depending on the action</param>
        public Time(Guild guild, TimeAction action, DateTimeOffset date, int interval, string additional)
        {
            // Inputs
            {
                this.guild = guild;
                this.action = action;
                this.date = date;
                this.interval = interval;
                this.additional = additional;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Indicates whether this instance of Time is allowed to be scanned
        /// </summary>
        public bool IsScanAllowed()
        {
            DateTimeOffset nowUtc = DateTimeOffset.UtcNow;
            DateTimeOffset nextUtc = Next().UtcDateTime;

            return nowUtc.CompareTo(nextUtc) > 0;
        }

        /// <summary>
        /// Gets next time value after the interval has been elapsed
        /// </summary>
        /// <returns>The next DateTimeOffset value after the interval has been elapsed</returns>
        public DateTimeOffset Next()
        {
            return date.AddSeconds(interval);
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

        public override bool Equals(object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Time? time = obj as Time;

                return Guild == time?.Guild && Action == time?.Action && Additional == time?.Additional;
            }
        }

        public override int GetHashCode()
        {
            return Guild.GetHashCode() ^ Action.GetHashCode() ^ Additional.GetHashCode();
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

        public static bool operator ==(Time? x, Time? y)
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

        public static bool operator !=(Time? x, Time? y)
        {
            return !(x == y);
        }
    }
}
