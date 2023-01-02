using Discord.Interactions;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Discord.Extensions;

namespace Wp.Bot.Modules.ComponentCommands.Admin
{
	public class Admin : InteractionModuleBase<SocketInteractionContext>
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

		public Admin(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.ADMIN_LANGUAGE_SELECT, runMode: RunMode.Async)]
		public async Task Language(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Recovers new language from selections
			Common.Models.Language dbLanguage = (Common.Models.Language)int.Parse(selections.First());

			// Updates
			dbGuild.Language = dbLanguage;
			Database.Context.Guilds.Update(dbGuild);

			await FollowupAsync(dbGuild.AdminText.AdminLanguageChanged, ephemeral: true);
		}
	}
}
