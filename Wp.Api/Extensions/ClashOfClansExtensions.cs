using ClashOfClans.Models;
using System.Web;

namespace Wp.Api.Extensions
{
    public static class ClashOfClansExtensions
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAN METHODS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets a link that opens directly the game on the clan profile
        /// </summary>
        /// <param name="clan">The clan</param>
        /// <param name="lang">A language, used for the language displayed on link</param>
        /// <returns>A link that opens directly the game on the clan profile</returns>
        public static string GetLink(this Clan clan, string lang)
        {
            return $"https://link.clashofclans.com/{lang}?action=OpenClanProfile&tag={HttpUtility.UrlEncode(clan.Tag)}";
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN WAR METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets all the attacks made during the clan war
        /// </summary>
        /// <param name="cWar">The clan war</param>
        /// <returns>A read-only collection containing all the attacks</returns>
        public static IReadOnlyCollection<ClanWarAttack> GetAllAttacks(this ClanWar cWar)
        {
            List<ClanWarAttack> attacks = new();

            // Clan attacks
            cWar.Clan?
                .Members?
                .AsParallel()
                .SelectMany(m => m.Attacks ?? Enumerable.Empty<ClanWarAttack>())
                .ToList()
                .ForEach(a => attacks.Add(a));

            // Opponent attacks
            cWar.Opponent?
                .Members?
                .AsParallel()
                .SelectMany(m => m.Attacks ?? Enumerable.Empty<ClanWarAttack>())
                .ToList()
                .ForEach(a => attacks.Add(a));

            return attacks.OrderBy(a => a.Order).ToArray();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PLAYER METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets a link that opens directly the game on the player profile
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="lang">A language, used for the language displayed on link</param>
        /// <returns>A link that opens directly the game on the player profile</returns>
        public static string GetLink(this Player player, string lang)
        {
            return $"https://link.clashofclans.com/{lang}?action=OpenPlayerProfile&tag={HttpUtility.UrlEncode(player.Tag)}";
        }
    }
}
