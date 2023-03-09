using Wp.Common.Models;

namespace Wp.Database.Services
{
    public class DbPlayerStatistics : List<PlayerStatistic>
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly object _lock = new();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public DbPlayerStatistics(PlayerStatistic[] playerStatistics)
        {
            lock (_lock)
            {
                playerStatistics
                    .ToList()
                    .ForEach(ps => base.Add(ps));
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Adds a player statistic to the database and saves it
        /// </summary>
        /// <param name="playerStatistic">The player statistic to be added to the database</param>
        public new void Add(PlayerStatistic playerStatistic)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                ctx.PlayerStatistics.Add(playerStatistic.ToEFModel());
                ctx.SaveChanges();

                base.Add(playerStatistic);
            }
        }

        /// <summary>
        /// Removes a player statistic from the database
        /// </summary>
        /// <param name="playerStatistic">The player statistic to be removed</param>
        /// <returns>true if player statistic is successfully removed; false otherwise</returns>
        public new bool Remove(PlayerStatistic playerStatistic)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                EFModels.PlayerStatistic? dbPlayerStatistic = ctx.PlayerStatistics.GetEFModel(playerStatistic);

                if (dbPlayerStatistic == null) return false;

                ctx.PlayerStatistics.Remove(dbPlayerStatistic);
                ctx.SaveChanges();

                return base.Remove(playerStatistic);
            }
        }

        /// <summary>
        /// Removes a player statistic from the database
        /// </summary>
        /// <param name="predicate">A delegate for the matching player statistic to be removed</param>
        /// <returns>true if player statistic is successfully removed; false otherwise</returns>
        public bool Remove(Predicate<PlayerStatistic> predicate)
        {
            PlayerStatistic? playerStatistic = Find(predicate);

            return playerStatistic != null && Remove(playerStatistic);
        }
    }
}
