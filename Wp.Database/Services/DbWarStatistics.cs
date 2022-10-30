using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

namespace Wp.Database.Services
{
	public class DbWarStatistics : List<WarStatistic>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly object _lock = new();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public DbWarStatistics(IEnumerable<WarStatistic> warStatistics)
		{
			warStatistics.ForEach(ws => base.Add(ws));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(WarStatistic warStatistic)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.WarStatistics.Add(warStatistic.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(warStatistic);
		}

		public new bool Remove(WarStatistic warStatistic)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.WarStatistic dbWarStatistic = ctx.WarStatistics.GetEFModel(warStatistic);
				ctx.WarStatistics.Remove(dbWarStatistic);
				ctx.SaveChanges();
			}

			return base.Remove(warStatistic);
		}

		public bool Remove(Predicate<WarStatistic> predicate)
		{
			WarStatistic? warStatistic = Find(predicate);

			return warStatistic != null && Remove(warStatistic);
		}
	}
}
