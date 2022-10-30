using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

namespace Wp.Database.Services
{
    public class DbCompetitions : List<Competition>
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly object _lock = new();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public DbCompetitions(IEnumerable<Competition> competitions)
        {
            competitions.ForEach(c => base.Add(c));
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public new void Add(Competition competition)
        {
            lock (_lock)
            {
                using EFModels.HEARC_P3Context ctx = new();

                ctx.Competitions.Add(competition.ToEFModel());
                ctx.SaveChanges();
            }

            base.Add(competition);
        }

        public new bool Remove(Competition competition)
        {
            lock (_lock)
            {
                using EFModels.HEARC_P3Context ctx = new();

                EFModels.Competition dbCompetition = ctx.Competitions.GetEFModel(competition);
                ctx.Competitions.Remove(dbCompetition);
                ctx.SaveChanges();
            }

            return base.Remove(competition);
        }

        public bool Remove(Predicate<Competition> predicate)
        {
            Competition? competition = Find(predicate);

            return competition != null && Remove(competition);
        }

        public void Update(Competition competition)
        {
            lock (_lock)
            {
                using EFModels.HEARC_P3Context ctx = new();

                EFModels.Competition dbCompetition = ctx.Competitions.GetEFModel(competition);

                dbCompetition.ResultId = competition.ResultId;
                dbCompetition.Name = competition.Name;
                dbCompetition.MainClan = competition.MainTag;
                dbCompetition.SecondClan = competition.SecondTag;

                ctx.Competitions.Update(dbCompetition);
                ctx.SaveChanges();
            }

            base.Remove(competition);
            base.Add(competition);
        }
    }
}
