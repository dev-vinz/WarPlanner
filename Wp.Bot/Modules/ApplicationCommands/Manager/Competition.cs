using Discord;
using Discord.Interactions;
using System.Text;
using Wp.Api;
using Wp.Bot.Modules.ApplicationCommands.AutoCompletion;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord;
using Wp.Discord.ComponentInteraction;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Manager
{
	[Group("competition", "Competition commands handler")]
	public class Competition : InteractionModuleBase<SocketInteractionContext>
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

		public Competition(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[SlashCommand("add", "Register a new competition within the guild", runMode: RunMode.Async)]
		public async Task Add([Summary("name", "A competition's name")] string name)
		{
			await DeferAsync(true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets command responses
			IManager commandText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			if (Encoding.UTF8.GetByteCount(name) != name.Length)
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = commandText.CompetitionNameContainsEmoji);

				return;
			}

			// Checks if there's at least one clan registered
			if (!dbClans.Any())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = commandText.NoClanRegisteredToRemove);

				return;
			}

			// Checks Clash Of Clans API
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

				return;
			}

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.COMPETITION_ADD_SELECT_MAIN_CLAN);

			dbClans
				.AsParallel()
				.ForAll(c =>
				{
					ClashOfClans.Models.Clan cClan = c.Profile;
					menuBuilder.AddOption(cClan.Name, c.Tag, c.Tag, CustomEmojis.ParseClanLevel(cClan.ClanLevel));
				});

			// Sort options by name
			menuBuilder.Options = menuBuilder.Options
				.AsParallel()
				.OrderBy(o => o.Label)
				.ToList();

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder);

			IUserMessage message = await ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = commandText.ChooseCompetitionMainClan;
				msg.Components = new(componentBuilder.Build());
			});

			// Registers informations into storage
			ComponentStorage storage = ComponentStorage.GetInstance();

			string[] datas = new[] { name };
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[SlashCommand("delete", "Delete an existing competition registered within the guild", runMode: RunMode.Async)]
		public async Task Delete()
		{
			await DeferAsync(true);

			// Loads databases infos
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Competition[] dbCompetitions = competitions
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets command responses
			IManager commandText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Checks if there's at least one competition registered
			if (!dbCompetitions.Any())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = commandText.NoCompetitionToDelete);

				return;
			}

			// Checks Clash Of Clans API
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

				return;
			}

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.COMPETITION_DELETE_SELECT);

			dbCompetitions
				.AsParallel()
				.ForAll(c =>
				{
					menuBuilder.AddOption(c.Name, c.Id.ToString(), emote: CustomEmojis.CocTrophy);
				});

			// Sort options by name
			menuBuilder.Options = menuBuilder.Options
				.AsParallel()
				.OrderBy(o => o.Label)
				.ToList();

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder);

			IUserMessage message = await ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = commandText.ChooseCompetitionToDelete;
				msg.Components = new(componentBuilder.Build());
			});
		}

		[SlashCommand("edit", "Edit an existing competition registered within the guild", runMode: RunMode.Async)]
		public async Task Edit([Summary("competition", "A registered competition"), Autocomplete(typeof(CompetitionAutocompleteHandler))] string competition)
		{
			await DeferAsync(true);

			ulong competitionId = ulong.Parse(competition);

			// Loads databases infos
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Competition dbCompetition = competitions
				.First(c => c.Id == competitionId && c.Guild == dbGuild);

			// Gets interaction texts
			IManager commandText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Checks Clash Of Clans API
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

				return;
			}

			// Edit name button
			ButtonBuilder nameButtonBuilder = new ButtonBuilder()
				.WithLabel(commandText.EditCompetitionName)
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(IdProvider.COMPETITION_EDIT_BUTTON_NAME);

			// Edit result channel
			ButtonBuilder resultButtonBuilder = new ButtonBuilder()
				.WithLabel(commandText.EditCompetitionResultChannel)
				.WithDisabled(true)
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(IdProvider.COMPETITION_EDIT_BUTTON_RESULT_CHANNEL);

			// Edit main clan
			ButtonBuilder mainButtonBuilder = new ButtonBuilder()
				.WithLabel(commandText.EditCompetitionMainClan)
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(IdProvider.COMPETITION_EDIT_BUTTON_MAIN_CLAN);

			// Edit second clan
			ButtonBuilder secondButtonBuilder = new ButtonBuilder()
				.WithLabel(commandText.EditCompetitionSecondClan)
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(IdProvider.COMPETITION_EDIT_BUTTON_SECOND_CLAN);

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithButton(nameButtonBuilder, 0)
				.WithButton(resultButtonBuilder, 0)
				.WithButton(mainButtonBuilder, 1)
				.WithButton(secondButtonBuilder, 1)
				.WithButton(cancelButtonBuilder, 2);

			IUserMessage message = await ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = commandText.EditCompetitionChooseEdition(dbCompetition.Name);
				msg.Components = new(componentBuilder.Build());
			});

			// Registers informations into storage
			ComponentStorage storage = ComponentStorage.GetInstance();

			string[] datas = new[] { competition };
			storage.MessageDatas.TryAdd(message.Id, datas);
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
