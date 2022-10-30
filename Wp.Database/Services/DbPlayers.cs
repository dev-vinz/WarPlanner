using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

namespace Wp.Database.Services
{
	public class DbPlayers : List<Player>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly object _lock = new();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public DbPlayers(IEnumerable<Player> players)
		{
			players.ForEach(p => base.Add(p));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(Player player)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.Players.Add(player.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(player);
		}

		public new bool Remove(Player player)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Player dbPlayer = ctx.Players.GetEFModel(player);
				ctx.Players.Remove(dbPlayer);
				ctx.SaveChanges();
			}

			return base.Remove(player);
		}

		public bool Remove(Predicate<Player> predicate)
		{
			Player? player = Find(predicate);

			return player != null && Remove(player);
		}
	}
}
