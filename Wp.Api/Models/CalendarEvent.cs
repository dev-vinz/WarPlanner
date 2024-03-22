using ClashOfClans.Models;

namespace Wp.Api.Models
{
    public class CalendarEvent
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly string id;
        private readonly string competitionName;
        private string opponentTag;
        private DateTimeOffset start;
        private DateTimeOffset end;
        private string[] players;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the event id
        /// </summary>
        public string Id
        {
            get => id;
        }

        /// <summary>
        /// Gets the match competition name
        /// </summary>
        public string CompetitionName
        {
            get => competitionName;
        }

        /// <summary>
        /// Gets the match opponent tag
        /// </summary>
        public string OpponentTag
        {
            get => opponentTag;
            set => opponentTag = value;
        }

        /// <summary>
        /// Gets the match start time
        /// </summary>
        public DateTimeOffset Start
        {
            get => start;
            set => start = value;
        }

        /// <summary>
        /// Gets the match end time
        /// </summary>
        public DateTimeOffset End
        {
            get => end;
            set => end = value;
        }

        /// <summary>
        /// Gets the match players
        /// </summary>
        public IReadOnlyCollection<string> Players
        {
            get => players;
            set => players = value.ToArray();
        }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the Clash Of Clans opponent clan profile via the API
        /// </summary>
        public Clan OpponentClan =>
            ClashOfClansApi.Clans.GetByTagAsync(opponentTag).Result
            ?? new Clan { Name = "[DELETED]", Tag = opponentTag, };

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents a war match inside a Google Calendar
        /// </summary>
        /// <param name="id">An event id</param>
        /// <param name="competitionName">A competition name</param>
        /// <param name="opponentTag">A clash of clans clan's tag</param>
        /// <param name="start">A start time</param>
        /// <param name="end">An end time</param>
        /// <param name="players">An array of players</param>
        public CalendarEvent(
            string id,
            string competitionName,
            string opponentTag,
            DateTimeOffset start,
            DateTimeOffset end,
            string[] players
        )
        {
            this.id = id;
            this.competitionName = competitionName;
            this.opponentTag = opponentTag;
            this.start = start;
            this.end = end;
            this.players = players;
        }

        /// <summary>
        /// Represents a war match inside a Google Calendar
        /// </summary>
        /// <param name="competitionName">A competition name</param>
        /// <param name="opponentTag">A clash of clans clan's tag</param>
        /// <param name="start">A start time</param>
        /// <param name="end">An end time</param>
        /// <param name="players">An array of players</param>
        public CalendarEvent(
            string competitionName,
            string opponentTag,
            DateTimeOffset start,
            DateTimeOffset end,
            string[] players
        )
            : this(string.Empty, competitionName, opponentTag, start, end, players) { }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Indicates if this instance is situated between two dates
        /// </summary>
        /// <param name="start">The first date to compare</param>
        /// <param name="end">The second date to compare</param>
        /// <returns>true if this instance is situated between the two dates; false otherwise</returns>
        public bool IsBetweenDate(DateTimeOffset start, DateTimeOffset end)
        {
            return this.start.UtcDateTime.Date >= start.UtcDateTime.Date
                && this.end.UtcDateTime.Date <= end.UtcDateTime.Date;
        }

        /// <summary>
        /// Indicates if this instance is valid
        /// </summary>
        /// <returns>true if this instance is valid; false otherwise</returns>
        public bool IsValid()
        {
            return competitionName is not null && opponentTag is not null && players.Any();
        }

        /// <summary>
        /// Validate the current instance and returns null if it's not valid
        /// </summary>
        /// <returns>The current instance if it's valid; null otherwise</returns>
        public CalendarEvent? Validate()
        {
            return IsValid() ? this : null;
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
