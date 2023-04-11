using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using Wp.Api;
using Wp.Api.Models;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Discord;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;
using Calendar = Wp.Common.Models.Calendar;

namespace Wp.Bot.Modules.ComponentCommands.Manager
{
	public class War : InteractionModuleBase<SocketInteractionContext>
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

		public War(CommandHandler handler)
		{
			this.handler = handler;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          BUTTON COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.WAR_ADD_BUTTON_LAST_NEXT_PLAYERS, runMode: RunMode.Async)]
		public async Task AddLastSkipPlayers()
		{
			// Simply redirect to select component, exactly the same interaction
			await AddLastPlayers(Array.Empty<string>());
		}

		[ComponentInteraction(IdProvider.WAR_ADD_BUTTON_NEXT_PLAYERS, runMode: RunMode.Async)]
		public async Task AddSkipPlayers()
		{
			// Simply redirect to select component, exactly the same interaction
			await AddPlayers(Array.Empty<string>());
		}

		[ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_ADD_PLAYER, runMode: RunMode.Async)]
		public async Task EditAddPlayer()
		{
			await RespondAsync("TODO", ephemeral: true);
		}

		[ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_DAY, runMode: RunMode.Async)]
		public async Task EditDay()
		{
			await RespondAsync("TODO", ephemeral: true);
		}

		[ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_FORMAT, runMode: RunMode.Async)]
		public async Task EditFormat()
		{
			await RespondAsync("TODO", ephemeral: true);
		}

		[ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_OPPONENT, runMode: RunMode.Async)]
		public async Task EditOpponent()
		{
			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Gets guild and interaction text
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			Calendar dbCalendar = Database.Context
				.Calendars
				.First(c => c.Guild == dbGuild);

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
			string eventId = datas[0];

			CalendarEvent warEvent = (await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId))!;

			ModalBuilder modalBuilder = new ModalBuilder()
				.WithTitle(interactionText.WarEditOpponentModalTitle)
				.WithCustomId(WarEditOpponentModal.ID)
				.AddTextInput(interactionText.WarEditOpponentModalField, WarEditOpponentModal.OPPONENT_TAG_ID, value: warEvent.OpponentTag);

			await RespondWithModalAsync(modalBuilder.Build());
		}

		[ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_REMOVE_PLAYER, runMode: RunMode.Async)]
		public async Task EditRemovePlayer()
		{
			await RespondAsync("TODO", ephemeral: true);
		}

		[ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_START_HOUR, runMode: RunMode.Async)]
		public async Task EditStartHour()
		{
			await RespondAsync("TODO", ephemeral: true);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ComponentInteraction(IdProvider.WAR_ADD_SELECT_COMPETITION, runMode: RunMode.Async)]
		public async Task AddCompetition(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Loads databases infos
			DbCompetitions competitions = Database.Context.Competitions;
			DbPlayers players = Database.Context.Players;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Player[] dbPlayers = players
				.AsParallel()
				.Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
				.ToArray();

			// Gets all guild members
			IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

			// Filters one more time
			Player[] availablePlayers = dbPlayers
				.Where(p =>
				{
					SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

					return user is not null && user.IsAPlayer() && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel;
				})
				.OrderByDescending(p => p.Account.TownHallLevel)
				.ThenBy(p => p.Account.Name)
				.ToArray();

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 3)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string opponentTag = datas[0];
			string totalTime = datas[1];
			string warDate = datas[2];

			int nbPages = (int)Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
			bool isLastPage = 1 == nbPages;

			ulong competitionId = ulong.Parse(selections.First());
			Common.Models.Competition dbCompetition = competitions
				.AsParallel()
				.First(c => c.Guild == dbGuild && c.Id == competitionId);

			// Filters for options
			availablePlayers = availablePlayers
				.Take(Settings.MAX_OPTION_PER_SELECT_MENU)
				.ToArray();

			// Build select menu
			string customSelectId = isLastPage ? IdProvider.WAR_ADD_SELECT_LAST_PLAYERS : IdProvider.WAR_ADD_SELECT_PLAYERS;
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(customSelectId)
				.WithMinValues(1)
				.WithMaxValues(availablePlayers.Length);

			availablePlayers
				.ToList()
				.ForEach(p =>
				{
					ClashOfClans.Models.Player cPlayer = p.Account;
					menuBuilder.AddOption(cPlayer.Name, p.Tag, p.Tag, CustomEmojis.ParseTownHallLevel(cPlayer.TownHallLevel));
				});

			// Next button
			ButtonBuilder nextButtonBuilder = new ButtonBuilder()
				.WithLabel(interactionText.WarAddCompetitionNextPlayers)
				.WithDisabled(isLastPage)
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(IdProvider.WAR_ADD_BUTTON_NEXT_PLAYERS);

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder)
				.WithButton(nextButtonBuilder);

			IUserMessage message = await FollowupAsync(interactionText.WarAddCompetitionSelectPlayers(dbCompetition.Name, nbPages), components: componentBuilder.Build(), ephemeral: true);

			// Inserts new datas
			datas = new[] { opponentTag, totalTime.ToString(), warDate, dbCompetition.Id.ToString(), "0" };
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.WAR_ADD_SELECT_HOUR, runMode: RunMode.Async)]
		public async Task AddHour(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Loads databases infos
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 3)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string opponentTag = datas[0];
			string totalTime = datas[1];

			int hours = int.Parse(selections.First());
			DateTimeOffset warDate = DateTimeOffset.Parse(datas[2]).AddHours(hours);

			CultureInfo cultureInfo = dbGuild.CultureInfo;

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.WAR_ADD_SELECT_MINUTES);

			Enumerable.Range(0, 4)
				.ToList()
				.ForEach(n => menuBuilder.AddOption(warDate.AddMinutes(n * 15).ToString("t", cultureInfo), (n * 15).ToString(), emote: CustomEmojis.CocClock));

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder);

			IUserMessage message = await FollowupAsync(interactionText.WarAddHourSelectMinutes(warDate.ToString("t", cultureInfo)), components: componentBuilder.Build(), ephemeral: true);

			// Inserts new datas
			datas = new[] { opponentTag, totalTime.ToString(), warDate.ToString() };
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.WAR_ADD_SELECT_MINUTES, runMode: RunMode.Async)]
		public async Task AddMinutes(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

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

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 3)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string opponentTag = datas[0];
			string totalTime = datas[1];

			int minutes = int.Parse(selections.First());
			DateTimeOffset warDate = DateTimeOffset.Parse(datas[2]).AddMinutes(minutes);

			CultureInfo cultureInfo = dbGuild.CultureInfo;

			// Build select menu
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(IdProvider.WAR_ADD_SELECT_COMPETITION);

			dbCompetitions
				.OrderBy(c => c.Name)
				.ToList()
				.ForEach(c => menuBuilder.AddOption(c.Name, c.Id.ToString(), emote: CustomEmojis.CocTrophy));

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder);

			IUserMessage message = await FollowupAsync(interactionText.WarAddMinutesSelectCompetition(warDate.ToString("t", cultureInfo)), components: componentBuilder.Build(), ephemeral: true);

			// Inserts new datas
			datas = new[] { opponentTag, totalTime.ToString(), warDate.ToString() };
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.WAR_ADD_SELECT_LAST_PLAYERS, runMode: RunMode.Async)]
		public async Task AddLastPlayers(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Loads databases infos
			DbCalendars calendars = Database.Context.Calendars;
			DbCompetitions competitions = Database.Context.Competitions;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 5)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string opponentTag = datas![0];
			int totalTime = int.Parse(datas[1]);
			ulong competitionId = ulong.Parse(datas[3]);
			int currentPage = int.Parse(datas[4]) + 1;
			string[] playersTag = datas.Skip(5).Concat(selections).ToArray();

			DateTimeOffset warDate = DateTimeOffset.Parse(datas[2]);
			DateTimeOffset endDate = warDate.AddMinutes(totalTime);
			CultureInfo cultureInfo = dbGuild.CultureInfo;

			// Filters for guild
			Common.Models.Calendar dbCalendar = calendars
				.AsParallel()
				.First(c => c.Guild == dbGuild);

			Common.Models.Competition dbCompetition = competitions
				.AsParallel()
				.First(c => c.Guild == dbGuild && c.Id == competitionId);

			ClashOfClans.Models.Clan? cOpponent = await ClashOfClansApi.Clans.GetByTagAsync(opponentTag);

			// Creates war match and inserts into calendar
			CalendarEvent calendarEvent = new(dbCompetition.Name, opponentTag, warDate, endDate, playersTag);
			Google.Apis.Calendar.v3.Data.Event? resultEvent = await GoogleCalendarApi.Events.InsertAsync(calendarEvent, dbGuild.TimeZone.AsAttribute().Zone.Id, dbCalendar.Id);

			if (resultEvent is not null)
			{
				await FollowupAsync(interactionText.WarAddLastPlayersMatchAdded(warDate.ToString("d", cultureInfo), cOpponent!.Name), ephemeral: true);
			}
			else
			{
				await FollowupAsync(interactionText.WarAddLastPlayersMatchProblem, ephemeral: true);
			}
		}


		[ComponentInteraction(IdProvider.WAR_ADD_SELECT_PLAYERS, runMode: RunMode.Async)]
		public async Task AddPlayers(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Gets SocketMessageComponent and original message
			SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
			SocketUserMessage msg = socket.Message;

			// Loads databases infos
			DbPlayers players = Database.Context.Players;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Player[] dbPlayers = players
				.AsParallel()
				.Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
				.ToArray();

			// Gets all guild members
			IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

			// Filters one more time
			Player[] availablePlayers = dbPlayers
				.Where(p =>
				{
					SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

					return user is not null && user.IsAPlayer() && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel;
				})
				.OrderByDescending(p => p.Account.TownHallLevel)
				.ThenBy(p => p.Account.Name)
				.ToArray();

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 5)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers datas
			string opponentTag = datas![0];
			string totalTime = datas[1];
			string warDate = datas[2];
			string competitionId = datas[3];
			int currentPage = int.Parse(datas[4]) + 1;
			string[] playersTag = datas.Skip(5).Concat(selections).ToArray();

			int nbPages = (int)Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
			bool isLastPage = currentPage + 1 == nbPages;

			// Filters for options
			availablePlayers = availablePlayers
				.Skip(currentPage * Settings.MAX_OPTION_PER_SELECT_MENU)
				.Take(Settings.MAX_OPTION_PER_SELECT_MENU)
				.ToArray();

			// Build select menu
			string customSelectId = isLastPage ? IdProvider.WAR_ADD_SELECT_LAST_PLAYERS : IdProvider.WAR_ADD_SELECT_PLAYERS;
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithCustomId(customSelectId)
				.WithMinValues(1)
				.WithMaxValues(availablePlayers.Length);

			availablePlayers
				.ToList()
				.ForEach(p =>
				{
					ClashOfClans.Models.Player cPlayer = p.Account;
					menuBuilder.AddOption(cPlayer.Name, p.Tag, p.Tag, CustomEmojis.ParseTownHallLevel(cPlayer.TownHallLevel));
				});

			// Next button
			string customButtonId = isLastPage ? IdProvider.WAR_ADD_BUTTON_LAST_NEXT_PLAYERS : IdProvider.WAR_ADD_BUTTON_NEXT_PLAYERS;
			string buttonText = isLastPage ? interactionText.WarAddPlayersEnd : interactionText.WarAddCompetitionNextPlayers;
			ButtonBuilder nextButtonBuilder = new ButtonBuilder()
				.WithLabel(buttonText)
				.WithDisabled(isLastPage && !playersTag.Any())
				.WithStyle(ButtonStyle.Secondary)
				.WithCustomId(customButtonId);

			// Cancel button
			ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
				.WithLabel(generalResponses.CancelButton)
				.WithStyle(ButtonStyle.Danger)
				.WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder)
				.WithButton(cancelButtonBuilder)
				.WithButton(nextButtonBuilder);

			IUserMessage message = await FollowupAsync(interactionText.WarAddPlayersSelectPlayers(currentPage + 1, nbPages, playersTag.Length), components: componentBuilder.Build(), ephemeral: true);

			// Inserts new datas
			datas = new[] { opponentTag, totalTime.ToString(), warDate, competitionId, currentPage.ToString() }.Concat(playersTag).ToArray();
			storage.MessageDatas.TryAdd(message.Id, datas);
		}

		[ComponentInteraction(IdProvider.WAR_DELETE_SELECT_EVENT, runMode: RunMode.Async)]
		public async Task Delete(string[] selections)
		{
			await Context.Interaction.DisableComponentsAsync(allComponents: true);

			// Recovers option
			string eventId = selections.First();

			// Loads databases infos
			DbCalendars calendars = Database.Context.Calendars;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Common.Models.Calendar dbCalendar = calendars
				.AsParallel()
				.First(c => c.Guild == dbGuild);

			// Gets interaction texts
			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Deletes from google calendar
			bool isDeleted = await GoogleCalendarApi.Events.DeleteAsync(dbCalendar.Id, eventId);

			if (!isDeleted)
			{
				await FollowupAsync(interactionText.WarDeleteCannotDelete, ephemeral: true);

				return;
			}

			await FollowupAsync(interactionText.WarDeleteMatchDeleted, ephemeral: true);
		}
	}
}
