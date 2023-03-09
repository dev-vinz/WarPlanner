using Wp.Common.Models;
using Wp.Common.Services.Extensions;

namespace Wp.Database.Services
{
    public class DbTimes : List<Time>
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly object _lock = new();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public DbTimes(Time[] times)
        {
            lock (_lock)
            {
                times
                    .ToList()
                    .ForEach(t => base.Add(t));
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Adds a time to the database and saves it
        /// </summary>
        /// <param name="time">The time to be added to the database</param>
        public new void Add(Time time)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                ctx.Times.Add(time.ToEFModel());
                ctx.SaveChanges();

                base.Add(time);
            }
        }

        /// <summary>
        /// Removes a time from the database
        /// </summary>
        /// <param name="time">The time to be removed</param>
        /// <returns>true if time is successfully removed; false otherwise</returns>
        public new bool Remove(Time time)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                EFModels.Time? dbTime = ctx.Times.GetEFModel(time);

                if (dbTime == null) return false;

                ctx.Times.Remove(dbTime);
                ctx.SaveChanges();

                return base.Remove(time);
            }
        }

        /// <summary>
        /// Removes a time from the database
        /// </summary>
        /// <param name="predicate">A delegate for the matching time to be removed</param>
        /// <returns>true if time is successfully removed; false otherwise</returns>
        public bool Remove(Predicate<Time> predicate)
        {
            Time? time = Find(predicate);

            return time != null && Remove(time);
        }

        /// <summary>
        /// Updates a time in the database
        /// </summary>
        /// <param name="time">The time to be updated</param>
        public void Update(Time time)
        {
            lock (_lock)
            {
                using EFModels.HeArcP3Context ctx = new();

                EFModels.Time? dbTime = ctx.Times.GetEFModel(time);

                if (dbTime == null) return;

                dbTime.Date = time.Date.TruncSeconds().UtcDateTime;
                dbTime.Additional = time.Additional;
                dbTime.Optional = time.Optional;

                ctx.Times.Update(dbTime);
                ctx.SaveChanges();

                base.Remove(time);
                base.Add(time);
            }
        }
    }
}
