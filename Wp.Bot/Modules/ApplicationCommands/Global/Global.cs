using Discord;
using Discord.Interactions;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Global
{
	public class Global : InteractionModuleBase<SocketInteractionContext>
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

		public Global(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[SlashCommand("infos", "Get some informations about the bot, and the links")]
		public async Task Informations()
		{
			await DeferAsync(true);

			// Loads databases infos
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets responses
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;
			IGlobal commandText = dbGuild.GlobalText;

			// Documentation button
			ButtonBuilder docButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.Documentation)
				.WithStyle(ButtonStyle.Link)
				.WithUrl(Utilities.GITBOOK_DOCUMENTATION)
				.WithEmote(new Emoji("📚"));

			// Support button
			ButtonBuilder supportButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.SupportServer)
				.WithStyle(ButtonStyle.Link)
				.WithUrl(Utilities.SUPPORT_GUILD_INVITATION)
				.WithEmote(new Emoji("⚙️"));

			// Link button
			ButtonBuilder linkButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.LinkInvitation)
				.WithStyle(ButtonStyle.Link)
				.WithUrl(Utilities.BOT_LINK_INVITATION)
				.WithEmote(new Emoji("🔗"));

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithButton(supportButtonBuilder)
				.WithButton(docButtonBuilder)
				.WithButton(linkButtonBuilder);

			// Embed
			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithTitle(commandText.EmbedInformations)
				.WithThumbnailUrl(Context.Guild.IconUrl)
				.WithDescription(commandText.EmbedDescription)
				.AddField(commandText.EmbedFieldAuthor, "Vincent Jeannin, ISC3il-b", true)
				.AddField(commandText.EmbedFieldYear, "2022 - 2023", true)
				.WithColor(new Color((uint)new Random().Next(0x1000000)))
				.WithFooter($"{dbGuild.Now.Year} © {Context.Client.CurrentUser.Username}");

			await ModifyOriginalResponseAsync(msg =>
			{
				msg.Components = new(componentBuilder.Build());
				msg.Embed = embedBuilder.Build();
			});
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



	}
}
