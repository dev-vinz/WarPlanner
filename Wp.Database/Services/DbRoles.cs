using Wp.Common.Models;

namespace Wp.Database.Services
{
	public class DbRoles : List<Role>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly object _lock = new();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public DbRoles(Role[] roles)
		{
			lock (_lock)
			{
				roles
					.ToList()
					.ForEach(r => base.Add(r));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Adds a role to the database and saves it
		/// </summary>
		/// <param name="role">The role to be added to the database</param>
		public new void Add(Role role)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.Roles.Add(role.ToEFModel());
				ctx.SaveChanges();

				base.Add(role);
			}
		}

		/// <summary>
		/// Removes a role from the database
		/// </summary>
		/// <param name="role">The role to be removed</param>
		/// <returns>true if role is successfully removed; false otherwise</returns>
		public new bool Remove(Role role)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Role dbRole = ctx.Roles.GetEFModel(role);
				ctx.Roles.Remove(dbRole);
				ctx.SaveChanges();

				return base.Remove(role);
			}
		}

		/// <summary>
		/// Removes a role from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching role to be removed</param>
		/// <returns>true if role is successfully removed; false otherwise</returns>
		public bool Remove(Predicate<Role> predicate)
		{
			Role? role = Find(predicate);

			return role != null && Remove(role);
		}
	}
}
