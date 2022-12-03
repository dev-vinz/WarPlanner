using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Wp.Api;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands.Manager
{
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
        |*                          BUTTON COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.COMPETITION_ADD_BUTTON_NO_SECOND_CLAN, runMode: RunMode.Async)]
		public async Task AddSkipSecondClan()
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			DbRoles roles = Database.Context.Roles;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 2)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string competitionName = datas[0];
			string mainTag = datas[1];

			// Guild roles
			IReadOnlyCollection<IRole> guildRoles = roles
				.AsParallel()
				.Where(r => r.Guild == dbGuild && r.Type == RoleType.PLAYER)
				.Select(r => Context.Guild.GetRole(r.Id))
				.ToArray();

			// Create environment
			if (!TryCreateEnvironment(competitionName, guildRoles, out ulong? categoryId, out ulong? resultId, out IRole? refRole))
			{
				await FollowupAsync(interactionText.CouldntCreateCompetitionEnvironment, ephemeral: true);

				return;
			}

			// Register competition
			Common.Models.Competition dbCompetition = new(dbGuild, categoryId!.Value, resultId!.Value, competitionName, mainTag);
			competitions.Add(dbCompetition);

			await FollowupAsync(interactionText.CompetitionAdded(competitionName, dbCompetition.MainClan.Name), ephemeral: true);
		}

		[ComponentInteraction(IdProvider.COMPETITION_EDIT_BUTTON_MAIN_CLAN, runMode: RunMode.Async)]
		public async Task EditMainClan()
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets guild and interaction text
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Filters for guild
			Common.Models.Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			ulong competitionId = ulong.Parse(datas[0]);

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.COMPETITION_EDIT_SELECT_MAIN_CLAN);

			dbClans
				.AsParallel()
				.ForAll(c =>
				{
					menuBuilder.AddOption(c.Profile.Name, c.Tag, c.Tag);
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

			IUserMessage message = await FollowupAsync(interactionText.EditCompetitionSelectMainClan, components: componentBuilder.Build(), ephemeral: true);

			// Adds datas to new message
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.COMPETITION_EDIT_BUTTON_SECOND_CLAN, runMode: RunMode.Async)]
		public async Task EditSecondClan()
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets guild and interaction text
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Filters for guild
			Common.Models.Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			ulong competitionId = ulong.Parse(datas[0]);

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.COMPETITION_EDIT_SELECT_SECOND_CLAN);

			dbClans
				.AsParallel()
				.ForAll(c =>
				{
					menuBuilder.AddOption(c.Profile.Name, c.Tag, c.Tag);
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

			IUserMessage message = await FollowupAsync(interactionText.EditCompetitionSelectSecondClan, components: componentBuilder.Build(), ephemeral: true);

			// Adds datas to new message
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.COMPETITION_EDIT_BUTTON_NAME, runMode: RunMode.Async)]
		public async Task EditName()
		{
			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets guild and interaction text
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryGetValue(msg.Id, out string[]? datas) && datas?.Length != 1)
			{
				await RespondAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			ulong competitionId = ulong.Parse(datas[0]);

			Common.Models.Competition dbCompetition = competitions
				.First(c => c.Id == competitionId && c.Guild == dbGuild);

			ModalBuilder modalBuilder = new ModalBuilder()
				.WithTitle(interactionText.EditCompetitionNameModalTitle)
				.WithCustomId(CompetitionEditNameModal.ID)
				.AddTextInput(interactionText.EditCompetitionNameModalField, CompetitionEditNameModal.NAME_ID, value: dbCompetition.Name);

			await RespondWithModalAsync(modalBuilder.Build());
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.COMPETITION_ADD_SELECT_MAIN_CLAN, runMode: RunMode.Async)]
		public async Task AddMainClanSelect(string[] selections)
		{
			string clanTag = selections.First();

			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			DbRoles roles = Database.Context.Roles;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			Common.Models.Competition[] dbCompetitions = competitions
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Checks Clash Of Clans API
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await FollowupAsync(generalResponses.ClashOfClansError, ephemeral: true);

				return;
			}

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			string competitionName = datas[0];

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.COMPETITION_ADD_SELECT_SECOND_CLAN);

			dbClans
				.AsParallel()
				.ForAll(c =>
				{
					menuBuilder.AddOption(c.Profile.Name, c.Tag, c.Tag);
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

			// None button
			ButtonBuilder noneButtonBuilder = new ButtonBuilder()
				.WithLabel(interactionText.CompetitionNoSecondClanButton)
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(IdProvider.COMPETITION_ADD_BUTTON_NO_SECOND_CLAN);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder)
				.WithButton(noneButtonBuilder);

			// Let's go for second clan
			IUserMessage message = await FollowupAsync(interactionText.ChooseCompetitionSecondClan, components: componentBuilder.Build(), ephemeral: true);

			// Registers informations into storage
			datas = new[] { competitionName, clanTag };
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.COMPETITION_ADD_SELECT_SECOND_CLAN, runMode: RunMode.Async)]
		public async Task AddSecondClanSelect(string[] selections)
		{
			string secondTag = selections.First();

			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			DbRoles roles = Database.Context.Roles;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Guild roles
			IReadOnlyCollection<IRole> guildRoles = roles
				.AsParallel()
				.Where(r => r.Guild == dbGuild && r.Type == RoleType.PLAYER)
				.Select(r => Context.Guild.GetRole(r.Id))
				.ToArray();

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 2)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string competitionName = datas[0];
			string mainTag = datas[1];

			// Creates environment
			if (!TryCreateEnvironment(competitionName, guildRoles, out ulong? categoryId, out ulong? resultId, out IRole? refRole))
			{
				await FollowupAsync(interactionText.CouldntCreateCompetitionEnvironment, ephemeral: true);

				return;
			}

			// Register competition
			Common.Models.Competition dbCompetition = new(dbGuild, categoryId!.Value, resultId!.Value, competitionName, mainTag)
			{
				SecondTag = secondTag,
			};
			competitions.Add(dbCompetition);

			await FollowupAsync(interactionText.CompetitionAdded(competitionName, dbCompetition.MainClan.Name, dbCompetition.SecondClan?.Name), ephemeral: true);
		}

		[ComponentInteraction(IdProvider.COMPETITION_DELETE_SELECT, runMode: RunMode.Async)]
		public async Task Delete(string[] selections)
		{
			ulong competitionId = ulong.Parse(selections.First());

			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Competition dbCompetition = competitions
				.First(c => c.Id == competitionId && c.Guild == dbGuild);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;

			// Gets environment
			IReadOnlyList<SocketTextChannel> channels = Context.Guild.Channels
				.Where(c => c.GetChannelType() == ChannelType.Text)
				.Select(c => c as SocketTextChannel)
				.Where(c => c is not null && c.CategoryId == dbCompetition.Id)
				.ToList()!;

			IReadOnlyList<SocketVoiceChannel> voices = Context.Guild.Channels
				.Where(c => c.GetChannelType() == ChannelType.Text)
				.Select(c => c as SocketVoiceChannel)
				.Where(c => c is not null && c.CategoryId == dbCompetition.Id)
				.ToList()!;

			SocketGuildChannel category = Context.Guild.GetChannel(dbCompetition.Id);

			IReadOnlyList<SocketRole> roles = Context.Guild.Roles
				.Where(c => c.Name.Contains(dbCompetition.Name))
				.ToList();

			// Deletes environment
			channels
				.AsParallel()
				.ForAll(async c => await c.DeleteAsync());

			voices
				.AsParallel()
				.ForAll(async c => await c.DeleteAsync());

			await category.DeleteAsync();

			roles
				.AsParallel()
				.ForAll(async r => await r.DeleteAsync());

			// Deletes from DB
			competitions.Remove(dbCompetition);

			await FollowupAsync(interactionText.CompetitionDeleted(dbCompetition.Name), ephemeral: true);
		}

		[ComponentInteraction(IdProvider.COMPETITION_EDIT_SELECT_MAIN_CLAN, runMode: RunMode.Async)]
		public async Task EditMainClan(string[] selections)
		{
			string clanTag = selections.First();

			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			ulong competitionId = ulong.Parse(datas[0]);

			// Updates competition
			Common.Models.Competition dbCompetition = competitions.First(c => c.Id == competitionId && c.Guild == dbGuild);
			dbCompetition.MainTag = clanTag;

			competitions.Update(dbCompetition);

			await FollowupAsync(interactionText.EditCompetitionSelectMainClanUpdated(dbCompetition.Name, dbCompetition.MainClan.Name), ephemeral: true);
		}

		[ComponentInteraction(IdProvider.COMPETITION_EDIT_SELECT_SECOND_CLAN, runMode: RunMode.Async)]
		public async Task EditSecondClan(string[] selections)
		{
			string clanTag = selections.First();

			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Loads databases infos
			DbClans clans = Database.Context.Clans;
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			ulong competitionId = ulong.Parse(datas[0]);

			// Updates competition
			Common.Models.Competition dbCompetition = competitions.First(c => c.Id == competitionId && c.Guild == dbGuild);
			dbCompetition.SecondTag = clanTag;

			competitions.Update(dbCompetition);

			await FollowupAsync(interactionText.EditCompetitionSelectSecondClanUpdated(dbCompetition.Name, dbCompetition.MainClan.Name), ephemeral: true);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private bool TryCreateEnvironment(string competitionName, IReadOnlyCollection<IRole> playerRoles, out ulong? categoryId, out ulong? resultId, out IRole? refRole)
		{
			try
			{
				// Gets main datas
				IGuild guild = Context.Guild;
				Guild dbGuild = Database.Context
					.Guilds
					.First(g => g.Id == Context.Guild.Id);

				// Gets interaction texts
				IManager interactionText = dbGuild.ManagerText;

				// Gets bot as a member
				IGuildUser botMember = guild.GetCurrentUserAsync().Result;

				/* * * * * * * * * * * * * * * *\
                |*        ROLE CREATION        *|
                \* * * * * * * * * * * * * * * */

				IRole referentRole = guild.CreateRoleAsync(interactionText.CompetitionEnvironmentReferentRoleName(competitionName), GuildPermissions.None, isMentionable: true).Result;
				IRole tournamentRole = guild.CreateRoleAsync(competitionName, GuildPermissions.None, isMentionable: true).Result;

				/* * * * * * * * * * * * * * * *\
                |*      CHANNELS CREATION      *|
                \* * * * * * * * * * * * * * * */

				IGuildChannel category = guild.CreateCategoryAsync($"🏆 {competitionName}").Result;
				IGuildChannel informations = guild.CreateTextChannelAsync(interactionText.CompetitionEnvironmentInformationChannel, c => c.CategoryId = category.Id).Result;
				IGuildChannel flood = guild.CreateTextChannelAsync(interactionText.CompetitionEnvironmentFloodChannel, c => c.CategoryId = category.Id).Result;
				IGuildChannel result = guild.CreateTextChannelAsync(interactionText.CompetitionenvironmentResultChannel, c => c.CategoryId = category.Id).Result;
				IGuildChannel voice = guild.CreateVoiceChannelAsync(interactionText.CompetitionEnvironmentVoiceChannel, c => c.CategoryId = category.Id).Result;

				/* * * * * * * * * * * * * * * *\
                |*     CHANNELS PERMISSION     *|
                \* * * * * * * * * * * * * * * */

				category.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
				category.AddPermissionOverwriteAsync(referentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
				category.AddPermissionOverwriteAsync(tournamentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
				category.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)).Wait();

				informations.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
				informations.AddPermissionOverwriteAsync(referentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
				informations.AddPermissionOverwriteAsync(tournamentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny)).Wait();
				informations.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)).Wait();

				result.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
				result.AddPermissionOverwriteAsync(referentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
				result.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny)).Wait();

				voice.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
				voice.AddPermissionOverwriteAsync(referentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, useVoiceActivation: PermValue.Allow, speak: PermValue.Allow)).Wait();
				voice.AddPermissionOverwriteAsync(tournamentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, useVoiceActivation: PermValue.Allow, speak: PermValue.Allow)).Wait();
				voice.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)).Wait();

				/* * * * * * * * * * * * * * * *\
                |*      RESULT PERMISSION      *|
                \* * * * * * * * * * * * * * * */

				playerRoles
					.AsParallel()
					.ForAll(async role => await result.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny)));

				/* * * * * * * * * * * * * * * *\
                |*         ADD REF ROLE        *|
                \* * * * * * * * * * * * * * * */

				Task<IReadOnlyCollection<IGuildUser>> users = guild.GetUsersAsync();

				users.Wait();

				users.Result
					.AsParallel()
					.Where(u => u.IsAManager())
					.ForAll(async u => await u.AddRoleAsync(referentRole));


				/* * * * * * * * * * * * * * * *\
                |*          RETURN IDS         *|
                \* * * * * * * * * * * * * * * */

				categoryId = category.Id;
				resultId = result.Id;
				refRole = referentRole;

				return true;
			}
			catch (Exception)
			{
				categoryId = null;
				resultId = null;
				refRole = null;

				return false;
			}
		}
	}
}
