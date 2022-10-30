using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

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

		public DbGuilds(IEnumerable<Guild> guilds)
		{
			guilds.ForEach(g => base.Add(g));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(Guild guild)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.Guilds.Add(guild.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(guild);
		}

		public new bool Remove(Guild guild)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Guild dbGuild = ctx.Guilds.GetEFModel(guild);
				ctx.Guilds.Remove(dbGuild);
				ctx.SaveChanges();
			}

			return base.Remove(guild);
		}

		public bool Remove(Predicate<Guild> predicate)
		{
			Guild? guild = Find(predicate);

			return guild != null && Remove(guild);
		}

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
			}

			base.Remove(guild);
			base.Add(guild);
		}
	}
}
