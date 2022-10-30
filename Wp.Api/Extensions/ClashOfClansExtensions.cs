using ClashOfClans.Models;

namespace Wp.Api.Extensions
{
    public static class ClashOfClansExtensions
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAN METHODS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets all the attacks made during the clan war
        /// </summary>
        /// <param name="cWar">The clan war</param>
        /// <returns>A read-only collection containing all the attacks</returns>
        public static IReadOnlyCollection<ClanWarAttack> GetAllAttacks(this ClanWar cWar)
        {
            List<ClanWarAttack> attacks = new List<ClanWarAttack>();

            // Clan attacks
            cWar
                .Clan?
                .Members?
                .AsParallel()
                .SelectMany(m => m.Attacks ?? Enumerable.Empty<ClanWarAttack>())
                .ToList()
                .ForEach(a => attacks.Add(a));

            // Opponent attacks
            cWar
                .Opponent?
                .Members?
                .AsParallel()
                .SelectMany(m => m.Attacks ?? Enumerable.Empty<ClanWarAttack>())
                .ToList()
                .ForEach(a => attacks.Add(a));

            return attacks.OrderBy(a => a.Order).ToArray();
        }
    }
}
