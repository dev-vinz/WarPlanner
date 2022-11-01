using Wp.Common.Models;
using Wp.Database.Services.Extensions;

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

		/// <summary>
		/// Adds a war statistic to the database and saves it
		/// </summary>
		/// <param name="warStatistic">The war statistic to be added to the database</param>
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

		/// <summary>
		/// Removes a war statistic from the database
		/// </summary>
		/// <param name="warStatistic">The war statistic to be removed</param>
		/// <returns>true if war statistic is successfully removed; false otherwise</returns>
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

		/// <summary>
		/// Removes a war statistic from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching war statistic to be removed</param>
		/// <returns>true if war statistic is successfully removed; false otherwise</returns>
		public bool Remove(Predicate<WarStatistic> predicate)
		{
			WarStatistic? warStatistic = Find(predicate);

			return warStatistic != null && Remove(warStatistic);
		}
	}
}
