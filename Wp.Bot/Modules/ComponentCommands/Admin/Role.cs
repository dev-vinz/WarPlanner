using Discord.Interactions;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands.Admin
{
	public class Role : InteractionModuleBase<SocketInteractionContext>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly CommandHandler handler;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public InteractionService? Commands { get; set; }

		/* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Role(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.ROLE_MANAGER_DELETE_SELECT, runMode: RunMode.Async)]
		public async Task ManagerDelete(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbRoles roles = Database.Context.Roles;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Recovers role id from selections
			ulong roleId = ulong.Parse(selections.First());

			// Deletes
			roles.Remove(r => r.Id == roleId && r.Type == RoleType.MANAGER);

			// Gets interaction responses
			IAdmin interactionText = dbGuild.AdminText;

			await FollowupAsync(interactionText.RoleManagerDeleteSelectRoleDeleted, ephemeral: true);
		}

		[ComponentInteraction(IdProvider.ROLE_PLAYER_DELETE_SELECT, runMode: RunMode.Async)]
		public async Task PlayerDelete(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbRoles roles = Database.Context.Roles;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Recovers role id from selections
			ulong roleId = ulong.Parse(selections.First());

			// Deletes
			roles.Remove(r => r.Id == roleId && r.Type == RoleType.PLAYER);

			// Gets interaction responses
			IAdmin interactionText = dbGuild.AdminText;

			await FollowupAsync(interactionText.RolePlayerDeleteSelectRoleDeleted, ephemeral: true);
		}
	}
}
