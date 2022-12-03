using Discord.Interactions;
using Discord.WebSocket;
using Wp.Api;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands.Manager
{
	public class Clan : InteractionModuleBase<SocketInteractionContext>
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

		public Clan(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.CLAN_REMOVE_SELECT_MENU, runMode: RunMode.Async)]
		public async Task RemoveSelect(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			string clanTag = selections.First();

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets clan
			ClashOfClans.Models.Clan? cClan = await ClashOfClansApi.Clans.GetByTagAsync(clanTag);

			if (cClan is null)
			{
				await FollowupAsync(generalResponses.ClashOfClansError, ephemeral: true);

				return;
			}

			// Removes clan
			clans.Remove(c => c.Guild == dbGuild && c.Tag == cClan.Tag);

			await FollowupAsync(interactionText.ClanSelectRemoved(cClan.Name), ephemeral: true);
		}
	}
}
