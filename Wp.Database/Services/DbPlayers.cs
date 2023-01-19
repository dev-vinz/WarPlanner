using Wp.Common.Models;

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

		public DbPlayers(Player[] players)
		{
			lock (_lock)
			{
				players
					.ToList()
					.ForEach(p => base.Add(p));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Adds a player to the database and saves it
		/// </summary>
		/// <param name="player">The player to be added to the database</param>
		public new void Add(Player player)
		{
			lock (_lock)
			{
				using EFModels.HeArcP3Context ctx = new();

				ctx.Players.Add(player.ToEFModel());
				ctx.SaveChanges();

				base.Add(player);
			}
		}

		/// <summary>
		/// Removes a player from the database
		/// </summary>
		/// <param name="player">The player to be removed</param>
		/// <returns>true if player is successfully removed; false otherwise</returns>
		public new bool Remove(Player player)
		{
			lock (_lock)
			{
				using EFModels.HeArcP3Context ctx = new();

				EFModels.Player dbPlayer = ctx.Players.GetEFModel(player);
				ctx.Players.Remove(dbPlayer);
				ctx.SaveChanges();

				return base.Remove(player);
			}
		}

		/// <summary>
		/// Removes a player from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching player to be removed</param>
		/// <returns>true if player is successfully removed; false otherwise</returns>
		public bool Remove(Predicate<Player> predicate)
		{
			Player? player = Find(predicate);

			return player != null && Remove(player);
		}
	}
}
