using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

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

		public DbRoles(IEnumerable<Role> roles)
		{
			roles.ForEach(r => base.Add(r));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(Role role)
		{
			lock (_lock)
			{
				using Context ctx = new();

				ctx.Roles.Add(role.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(role);
		}

		public new bool Remove(Role role)
		{
			lock (_lock)
			{
				using Context ctx = new();

				EFModels.Role dbRole = ctx.Roles.GetEFModel(role);
				ctx.Roles.Remove(dbRole);
				ctx.SaveChanges();
			}

			return base.Remove(role);
		}

		public bool Remove(Predicate<Role> predicate)
		{
			Role? role = Find(predicate);

			return role != null && Remove(role);
		}
	}
}
