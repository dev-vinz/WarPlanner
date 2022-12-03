using Wp.Common.Models;

namespace Wp.Database.Services
{
	public class DbGuilds : List<Guild>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly object _lock = new();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public DbGuilds(Guild[] guilds)
		{
			lock (_lock)
			{
				guilds
					.ToList()
					.ForEach(g => base.Add(g));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Adds a guild to the database and saves it
		/// </summary>
		/// <param name="guild">The guild to be added to the database</param>
		public new void Add(Guild guild)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.Guilds.Add(guild.ToEFModel());
				ctx.SaveChanges();

				base.Add(guild);
			}
		}

		/// <summary>
		/// Removes a guild from the database
		/// </summary>
		/// <param name="guild">The guild to be removed</param>
		/// <returns>true if guild is successfully removed; false otherwise</returns>
		public new bool Remove(Guild guild)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Guild dbGuild = ctx.Guilds.GetEFModel(guild);
				ctx.Guilds.Remove(dbGuild);
				ctx.SaveChanges();

				return base.Remove(guild);
			}
		}

		/// <summary>
		/// Removes a guild from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching guild to be removed</param>
		/// <returns>true if guild is successfully removed; false otherwise</returns>
		public bool Remove(Predicate<Guild> predicate)
		{
			Guild? guild = Find(predicate);

			return guild != null && Remove(guild);
		}

		/// <summary>
		/// Updates a guild in the database
		/// </summary>
		/// <param name="guild">The guild to be updated</param>
		public void Update(Guild guild)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Guild dbGuild = ctx.Guilds.GetEFModel(guild);

				dbGuild.Language = (int)guild.Language;
				dbGuild.TimeZone = (int)guild.TimeZone;
				dbGuild.PremiumLevel = (int)guild.PremiumLevel;
				dbGuild.MinThlevel = (int)guild.MinimalTownHallLevel;

				ctx.Guilds.Update(dbGuild);
				ctx.SaveChanges();

				base.Remove(guild);
				base.Add(guild);
			}
		}
	}
}
