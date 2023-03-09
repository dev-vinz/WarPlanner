using Wp.Common.Models;

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

        public DbCompetitions(Competition[] competitions)
        {
            lock (_lock)
            {
                competitions
                    .ToList()
                    .ForEach(c => base.Add(c));
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
		/// Adds a competition to the database and saves it
		/// </summary>
		/// <param name="competition">The competition to be added to the database</param>
        public new void Add(Competition competition)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                ctx.Competitions.Add(competition.ToEFModel());
                ctx.SaveChanges();

                base.Add(competition);
            }
        }

        /// <summary>
		/// Removes a competition from the database
		/// </summary>
		/// <param name="competition">The competition to be removed</param>
		/// <returns>true if competition is successfully removed; false otherwise</returns>
        public new bool Remove(Competition competition)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                EFModels.Competition? dbCompetition = ctx.Competitions.GetEFModel(competition);

                if (dbCompetition == null) return false;

                ctx.Competitions.Remove(dbCompetition);
                ctx.SaveChanges();

                return base.Remove(competition);
            }
        }

        /// <summary>
		/// Removes a competition from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching competition to be removed</param>
		/// <returns>true if competition is successfully removed; false otherwise</returns>
        public bool Remove(Predicate<Competition> predicate)
        {
            Competition? competition = Find(predicate);

            return competition != null && Remove(competition);
        }

        /// <summary>
		/// Updates a competition in the database
		/// </summary>
		/// <param name="competition">The competition to be updated</param>
        public void Update(Competition competition)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                EFModels.Competition? dbCompetition = ctx.Competitions.GetEFModel(competition);

                if (dbCompetition == null) return;

                dbCompetition.ResultId = competition.ResultId;
                dbCompetition.Name = competition.Name;
                dbCompetition.MainClan = competition.MainTag;
                dbCompetition.SecondClan = competition.SecondTag;

                ctx.Competitions.Update(dbCompetition);
                ctx.SaveChanges();

                base.Remove(competition);
                base.Add(competition);
            }
        }
    }
}
