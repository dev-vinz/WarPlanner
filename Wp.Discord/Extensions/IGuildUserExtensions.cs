using Discord;
using Wp.Common.Models;
using Wp.Database;
using Wp.Database.Services;

namespace Wp.Discord.Extensions
{
	public static class IGuildUserExtensions
	{
		/// <summary>
		/// Indicates wether a guild user is an admin or not
		/// </summary>
		/// <param name="user">An user to be check</param>
		/// <returns>True if the user is an admin; False otherwise</returns>
		public static bool IsAnAdmin(this IGuildUser user)
		{
			return user.GuildPermissions.Has(GuildPermission.Administrator) && !user.IsBot;
		}

		/// <summary>
		/// Indicates wether a guild user is a manager or not
		/// </summary>
		/// <param name="user">An user to be check</param>
		/// <returns>True if the user is a manager; False otherwise</returns>
		public static bool IsAManager(this IGuildUser user)
		{
			// Loads databases infos
			DbRoles roles = Context.Roles;
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == user.GuildId);

			// Filters for guild
			Role[] dbRoles = roles
				.AsParallel()
				.Where(r => r.Guild == dbGuild && r.Type == RoleType.MANAGER)
				.ToArray();

			return (user.RoleIds.Any(id => dbRoles.Any(r => r.Id == id)) || user.IsAnAdmin()) && !user.IsBot;
		}

		/// <summary>
		/// Indicates wether a guild user is a player or not
		/// </summary>
		/// <param name="user">An user to be check</param>
		/// <returns>True if the user is a player; False otherwise</returns>
		public static bool IsAPlayer(this IGuildUser user)
		{
			// Loads databases infos
			DbRoles roles = Context.Roles;
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == user.GuildId);

			// Filters for guild
			Role[] dbRoles = roles
				.AsParallel()
				.Where(r => r.Guild == dbGuild && r.Type == RoleType.PLAYER)
				.ToArray();

			return (user.RoleIds.Any(id => dbRoles.Any(r => r.Id == id)) || user.IsAManager()) && !user.IsBot;
		}
	}
}
