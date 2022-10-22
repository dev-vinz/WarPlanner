using Wp.Api;

namespace Wp.Database.Models
{
    public class PlayerStatistic
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly Guild guild;
        private readonly ulong playerId;
        private readonly string playerTag;
        private readonly string clanTag;
        private readonly DateTimeOffset date;
        private readonly int order;
        private readonly WarType warType;
        private readonly PlayerStatisticType type;
        private readonly PlayerStatisticAction action;
        private readonly int stars;
        private readonly int percent;
        private readonly int duration;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the discord server associated at this instance
        /// </summary>
        public Guild Guild { get => guild; }

        /// <summary>
        /// Gets the player's discord id
        /// </summary>
        public ulong PlayerId { get => playerId; }

        /// <summary>
        /// Gets the player's Clash Of Clans tag
        /// </summary>
        public string PlayerTag { get => playerTag; }

        /// <summary>
        /// Gets the Clash Of Clans clan's tag where the statistic has been made
        /// </summary>
        public string ClanTag { get => clanTag; }

        /// <summary>
        /// Gets the statistic war's date
        /// </summary>
        public DateTimeOffset Date { get => date; }

        /// <summary>
        /// Gets the statistic's order during war
        /// </summary>
        public int Order { get => order; }

        /// <summary>
        /// Gets the war's type
        /// </summary>
        public WarType WarType { get => warType; }

        /// <summary>
        /// Gets the statistic's type
        /// </summary>
        public PlayerStatisticType Type { get => type; }

        /// <summary>
        /// Gets the statistic's action
        /// </summary>
        public PlayerStatisticAction Action { get => action; }

        /// <summary>
        /// Gets the number of stars made
        /// </summary>
        public int Stars { get => stars; }

        /// <summary>
        /// Gets the percentage made
        /// </summary>
        public int Percent { get => percent; }

        /// <summary>
        /// Gets the statistic's duration
        /// </summary>
        public int Duration { get => duration; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the Clash Of Clans player's account, via the API
        /// </summary>
        public ClashOfClans.Models.Player? Account => ClashOfClansApi.Players.GetByTagAsync(playerTag).Result;

        /// <summary>
        /// Gets the Clash Of Clans clan's profile, via the API
        /// </summary>
        public ClashOfClans.Models.Clan? Clan => ClashOfClansApi.Clans.GetByTagAsync(clanTag).Result;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents an attack or a defense from a Clash Of Clans player inside a discord server
        /// </summary>
        /// <param name="guild">A discord server, represented by the Guild object</param>
        /// <param name="playerId">A player's discord id</param>
        /// <param name="playerTag">A player's Clash Of Clans tag</param>
        /// <param name="clanTag">A clan's Clash Of Clans tag</param>
        /// <param name="date">A date when the attack or defense has been made</param>
        /// <param name="order">A Clash Of Clans attack's order during the war</param>
        /// <param name="warType">A Clash Of Clans war's type</param>
        /// <param name="type">A specification of the statistic</param>
        /// <param name="action">A specification of the attack or defense</param>
        /// <param name="stars">A number of stars made</param>
        /// <param name="percent">A percentage made</param>
        /// <param name="duration">A duration made</param>
        public PlayerStatistic(Guild guild, ulong playerId, string playerTag, string clanTag, DateTimeOffset date, int order, WarType warType, PlayerStatisticType type, PlayerStatisticAction action, int stars, int percent, int duration)
        {
            // Inputs
            {
                this.guild = guild;
                this.playerId = playerId;
                this.playerTag = playerTag;
                this.clanTag = clanTag;
                this.date = date;
                this.order = order;
                this.warType = warType;
                this.type = type;
                this.action = action;
                this.stars = stars;
                this.percent = percent;
                this.duration = duration;
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
                PlayerStatistic? playerStatistic = obj as PlayerStatistic;

                return PlayerId == playerStatistic?.PlayerId &&
                       ClanTag == playerStatistic?.ClanTag &&
                       Date == playerStatistic?.Date &&
                       Order == playerStatistic?.Order;
            }
        }

        public override int GetHashCode()
        {
            return PlayerId.GetHashCode() ^ ClanTag.GetHashCode() ^ Date.GetHashCode() ^ Order.GetHashCode();
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

        public static bool operator ==(PlayerStatistic? x, PlayerStatistic? y)
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

        public static bool operator !=(PlayerStatistic? x, PlayerStatistic? y)
        {
            return !(x == y);
        }
    }
}
