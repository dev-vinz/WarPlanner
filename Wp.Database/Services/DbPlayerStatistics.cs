using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

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

		public DbPlayerStatistics(IEnumerable<PlayerStatistic> playerStatistics)
		{
			playerStatistics.ForEach(ps => base.Add(ps));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(PlayerStatistic playerStatistic)
		{
			lock (_lock)
			{
				using Context ctx = new();

				ctx.PlayerStatistics.Add(playerStatistic.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(playerStatistic);
		}

		public new bool Remove(PlayerStatistic playerStatistic)
		{
			lock (_lock)
			{
				using Context ctx = new();

				EFModels.PlayerStatistic dbPlayerStatistic = ctx.PlayerStatistics.GetEFModel(playerStatistic);
				ctx.PlayerStatistics.Remove(dbPlayerStatistic);
				ctx.SaveChanges();
			}

			return base.Remove(playerStatistic);
		}

		public bool Remove(Predicate<PlayerStatistic> predicate)
		{
			PlayerStatistic? playerStatistic = Find(predicate);

			return playerStatistic != null && Remove(playerStatistic);
		}
	}
}
