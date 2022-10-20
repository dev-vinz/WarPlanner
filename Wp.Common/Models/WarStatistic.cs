using Wp.Api;

namespace Wp.Common.Models
{
    public class WarStatistic
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly Guild guild;
        private readonly DateTimeOffset date;
        private readonly WarType type;
        private readonly string clanTag;
        private readonly ulong? competitionId;
        private readonly string opponentName;
        private readonly WarResult result;
        private readonly int attackStars;
        private readonly double attackPercent;
        private readonly double attackAvgDuration;
        private readonly int defenseStars;
        private readonly double defensePercent;
        private readonly double defenseAvgDuration;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the discord server associated at this instance
        /// </summary>
        public Guild Guild { get => guild; }

        /// <summary>
        /// Gets the war's starting date
        /// </summary>
        public DateTimeOffset Date { get => date; }

        /// <summary>
        /// Gets the Clash Of Clans war's type
        /// </summary>
        public WarType Type { get => type; }

        /// <summary>
        /// Gets the Clash Of Clans clan's tag
        /// </summary>
        public string ClanTag { get => clanTag; }

        /// <summary>
        /// Gets the discord category's id associated at a competition
        /// </summary>
        public ulong? CompetitionId { get => competitionId; }

        /// <summary>
        /// Gets the Clash Of Clans opponent clan's name
        /// </summary>
        public string OpponentName { get => opponentName; }

        /// <summary>
        /// Gets the Clash Of Clans war's result
        /// </summary>
        public WarResult Result { get => result; }

        /// <summary>
        /// Gets the number of stars made in attack
        /// </summary>
        public int AttackStars { get => attackStars; }

        /// <summary>
        /// Gets the total percentage made in attack
        /// </summary>
        public double AttackPercent { get => attackPercent; }

        /// <summary>
        /// Gets the average attack duration
        /// </summary>
        public double AttackAvgDuration { get => attackAvgDuration; }

        /// <summary>
        /// Gets the number of stars made in defense
        /// </summary>
        public int DefenseStars { get => defenseStars; }

        /// <summary>
        /// Gets the total percentage made in defense
        /// </summary>
        public double DefensePercent { get => defensePercent; }

        /// <summary>
        /// Gets the average defense duration
        /// </summary>
        public double DefenseAvgDuration { get => defenseAvgDuration; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the Clash Of Clans clan's profile, via the API
        /// </summary>
        public ClashOfClans.Models.Clan? Clan => ClashOfClansApi.Clans.GetByTagAsync(clanTag).Result;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents a Clash Of Clans war inside a discord server
        /// </summary>
        /// <param name="guild">A discord server, represented by the Guild object</param>
        /// <param name="date">A date when the war has started</param>
        /// <param name="type">A Clash Of Clans war's type</param>
        /// <param name="clanTag">A Clash Of Clans clan's tag</param>
        /// <param name="competitionId">A discord category's id</param>
        /// <param name="opponentName">A Clash Of Clans clan's name</param>
        /// <param name="result">A Clash Of Clans war's result</param>
        /// <param name="attackStars">A number of total stars made in attack</param>
        /// <param name="attackPercent">A total percentage made in attack</param>
        /// <param name="attackAvgDuration">An average attack duration</param>
        /// <param name="defenseStars">A number of total stars made in defense</param>
        /// <param name="defensePercent">A total percentage made in defense</param>
        /// <param name="defenseAvgDuration">An average defense duration</param>
        public WarStatistic(Guild guild, DateTimeOffset date, WarType type, string clanTag, ulong? competitionId, string opponentName, WarResult result, int attackStars, double attackPercent, int attackAvgDuration, int defenseStars, double defensePercent, int defenseAvgDuration)
        {
            // Inputs
            {
                this.guild = guild;
                this.date = date;
                this.type = type;
                this.clanTag = clanTag;
                this.competitionId = competitionId;
                this.opponentName = opponentName;
                this.result = result;
                this.attackStars = attackStars;
                this.attackPercent = attackPercent;
                this.attackAvgDuration = attackAvgDuration;
                this.defenseStars = defenseStars;
                this.defensePercent = defensePercent;
                this.defenseAvgDuration = defenseAvgDuration;
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
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                WarStatistic? warStatistic = obj as WarStatistic;

                return Date == warStatistic?.Date && ClanTag == warStatistic?.ClanTag;
            }
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode() ^ ClanTag.GetHashCode();
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

        public static bool operator ==(WarStatistic? x, WarStatistic? y)
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

        public static bool operator !=(WarStatistic? x, WarStatistic? y)
        {
            return !(x == y);
        }
    }
}
