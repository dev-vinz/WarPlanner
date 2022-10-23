using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

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

		public DbTimes(IEnumerable<Time> times)
		{
			times.ForEach(t => base.Add(t));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(Time time)
		{
			lock (_lock)
			{
				using Context ctx = new();

				ctx.Times.Add(time.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(time);
		}

		public new bool Remove(Time time)
		{
			lock (_lock)
			{
				using Context ctx = new();

				EFModels.Time dbTime = ctx.Times.GetEFModel(time);
				ctx.Times.Remove(dbTime);
				ctx.SaveChanges();
			}

			return base.Remove(time);
		}

		public bool Remove(Predicate<Time> predicate)
		{
			Time? time = Find(predicate);

			return time != null && Remove(time);
		}

		public void Update(Time time)
		{
			lock (_lock)
			{
				using Context ctx = new();

				EFModels.Time dbTime = ctx.Times.GetEFModel(time);

				dbTime.Date = time.Date.UtcDateTime;
				dbTime.Additional = time.Additional;
				dbTime.Optional = time.Optional;

				ctx.Times.Update(dbTime);
				ctx.SaveChanges();
			}

			base.Remove(time);
			base.Add(time);
		}
	}
}
