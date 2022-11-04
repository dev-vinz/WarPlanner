using Wp.Common.Models;

namespace Wp.Database.Services
{
    public class DbClans : List<Clan>
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly object _lock = new();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public DbClans(IEnumerable<Clan> clans)
        {
            clans
                .AsParallel()
                .ForAll(c => base.Add(c));
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Adds a clan to the database and saves it
        /// </summary>
        /// <param name="clan">The clan to be added to the database</param>
        public new void Add(Clan clan)
        {
            lock (_lock)
            {
                using EFModels.HEARC_P3Context ctx = new();

                ctx.Clans.Add(clan.ToEFModel());
                ctx.SaveChanges();
            }

            base.Add(clan);
        }

        /// <summary>
		/// Removes a clan from the database
		/// </summary>
		/// <param name="clan">The clan to be removed</param>
		/// <returns>true if clan is successfully removed; false otherwise</returns>
        public new bool Remove(Clan clan)
        {
            lock (_lock)
            {
                using EFModels.HEARC_P3Context ctx = new();

                EFModels.Clan dbClan = ctx.Clans.GetEFModel(clan);
                ctx.Clans.Remove(dbClan);
                ctx.SaveChanges();
            }

            return base.Remove(clan);
        }

        /// <summary>
		/// Removes a clan from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching clan to be removed</param>
		/// <returns>true if clan is successfully removed; false otherwise</returns>
        public bool Remove(Predicate<Clan> predicate)
        {
            Clan? clan = Find(predicate);

            return clan != null && Remove(clan);
        }
    }
}
