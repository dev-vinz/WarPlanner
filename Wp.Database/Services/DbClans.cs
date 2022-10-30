using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

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
            clans.ForEach(c => base.Add(c));
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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

        public bool Remove(Predicate<Clan> predicate)
        {
            Clan? clan = Find(predicate);

            return clan != null && Remove(clan);
        }
    }
}
