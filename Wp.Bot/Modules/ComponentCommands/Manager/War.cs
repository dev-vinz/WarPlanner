using System.Globalization;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Wp.Api;
using Wp.Api.Models;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Services.Extensions;
using Wp.Common.Services.NodaTime;
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
        public async Task EditAddPlayers()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Filters for guild
            Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .ToArray();

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers data
            string eventId = datas[0];
            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Gets all guild members
            IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

            // Filters one more time
            Player[] availablePlayers = dbPlayers
                .Where(p =>
                {
                    SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

                    return user is not null
                        && user.IsAPlayer()
                        && user.Roles.Any(
                            r =>
                                r.Name.Contains(
                                    warEvent.CompetitionName,
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                        )
                        && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel
                        && !warEvent.Players.Contains(
                            p.Tag,
                            StringComparer.InvariantCultureIgnoreCase
                        );
                })
                .OrderByDescending(p => p.Account.TownHallLevel)
                .ThenBy(p => p.Account.Name)
                .ToArray();

            int nbPages = (int)
                Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
            bool isLastPage = nbPages < 2;

            // Filters for options
            availablePlayers = availablePlayers.Take(Settings.MAX_OPTION_PER_SELECT_MENU).ToArray();

            // Build select menu
            string customSelectId = isLastPage
                ? IdProvider.WAR_EDIT_SELECT_ADD_LAST_PLAYERS
                : IdProvider.WAR_EDIT_SELECT_ADD_PLAYERS;
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(customSelectId)
                .WithMinValues(1)
                .WithMaxValues(availablePlayers.Length);

            availablePlayers
                .ToList()
                .ForEach(p =>
                {
                    ClashOfClans.Models.Player cPlayer = p.Account;
                    menuBuilder.AddOption(
                        cPlayer.Name,
                        p.Tag,
                        p.Tag,
                        CustomEmojis.ParseTownHallLevel(cPlayer.TownHallLevel)
                    );
                });

            // Next button
            string customButtonId = isLastPage
                ? IdProvider.WAR_EDIT_BUTTON_ADD_LAST_NEXT_PLAYERS
                : IdProvider.WAR_EDIT_BUTTON_ADD_NEXT_PLAYERS;
            string buttonText = isLastPage
                ? interactionText.WarEditAddPlayersEnd
                : interactionText.WarEditAddPlayersNext;
            ButtonBuilder nextButtonBuilder = new ButtonBuilder()
                .WithLabel(buttonText)
                .WithDisabled(isLastPage)
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

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditAddPlayersSelect(nbPages),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { eventId, "0" };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_ADD_NEXT_PLAYERS, runMode: RunMode.Async)]
        public async Task EditAddSkipPlayers()
        {
            // Simply redirect to select component, exactly the same interaction
            await EditAddPlayers(Array.Empty<string>());
        }

        [ComponentInteraction(
            IdProvider.WAR_EDIT_BUTTON_ADD_LAST_NEXT_PLAYERS,
            runMode: RunMode.Async
        )]
        public async Task EditAddSkipLastPlayers()
        {
            // Simply redirect to select component, exactly the same interaction
            await EditAddLastPlayers(Array.Empty<string>());
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_DAY, runMode: RunMode.Async)]
        public async Task EditDay()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers data
            string eventId = datas[0];
            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Gets date and culture info
            DateTimeOffset today = new(dbGuild.Now.Date, dbGuild.Now.Offset);
            CultureInfo cultureInfo = dbGuild.CultureInfo;

            NodaConverter converter = new();
            DateTimeOffset warDate = converter.ConvertDateTo(warEvent.Start, dbGuild.TimeZone);

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(
                IdProvider.WAR_EDIT_SELECT_DAY
            );

            Emoji calendar = new("🗓");

            Enumerable
                .Range(0, Settings.NB_WAR_PROPOSED_DATES)
                .ToList()
                .ForEach(n =>
                {
                    DateTimeOffset day = today.AddDays(n);
                    bool isDay = day.Date == warDate.Date;

                    menuBuilder.AddOption(
                        day.ToString("d", cultureInfo),
                        day.ToString(),
                        day.ToString("dddd", cultureInfo).Capitalize(),
                        calendar,
                        isDay
                    );
                });

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditDaySelect,
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { eventId };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_FORMAT, runMode: RunMode.Async)]
        public async Task EditFormat()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Build options
            List<SelectMenuOptionBuilder> options =
                new()
                {
                    new SelectMenuOptionBuilder($"5 {generalResponses.Minutes}", "5"),
                    new SelectMenuOptionBuilder($"15 {generalResponses.Minutes}", "15"),
                    new SelectMenuOptionBuilder($"30 {generalResponses.Minutes}", "30"),
                    new SelectMenuOptionBuilder($"1 {generalResponses.Hour}", "60"),
                    new SelectMenuOptionBuilder($"2 {generalResponses.Hours}", "120"),
                    new SelectMenuOptionBuilder($"4 {generalResponses.Hours}", "240"),
                    new SelectMenuOptionBuilder($"6 {generalResponses.Hours}", "360"),
                    new SelectMenuOptionBuilder($"8 {generalResponses.Hours}", "480"),
                    new SelectMenuOptionBuilder($"12 {generalResponses.Hours}", "720"),
                    new SelectMenuOptionBuilder($"16 {generalResponses.Hours}", "960"),
                    new SelectMenuOptionBuilder($"20 {generalResponses.Hours}", "1200"),
                    new SelectMenuOptionBuilder($"1 {generalResponses.Day}", "1440"),
                };

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.WAR_EDIT_SELECT_FORMAT_PREPARATION)
                .WithOptions(options);

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditFormatSelect,
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_OPPONENT, runMode: RunMode.Async)]
        public async Task EditOpponent()
        {
            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Gets guild and interaction text
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (
                !storage.MessageDatas.TryGetValue(msg.Id, out string[]? datas)
                && datas?.Length != 1
            )
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers data
            string eventId = datas[0];

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            ModalBuilder modalBuilder = new ModalBuilder()
                .WithTitle(interactionText.WarEditOpponentModalTitle)
                .WithCustomId(WarEditOpponentModal.ID)
                .AddTextInput(
                    interactionText.WarEditOpponentModalField,
                    WarEditOpponentModal.OPPONENT_TAG_ID,
                    value: warEvent.OpponentTag
                );

            await RespondWithModalAsync(modalBuilder.Build());
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_REMOVE_PLAYER, runMode: RunMode.Async)]
        public async Task EditRemovePlayer()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers data
            string eventId = datas[0];
            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Filters one more time
            ClashOfClans.Models.Player?[] availablePlayers = warEvent
                .Players.Select(p =>
                {
                    Task<ClashOfClans.Models.Player?> playerTask =
                        ClashOfClansApi.Players.GetByTagAsync(p);
                    playerTask.Wait();

                    return playerTask.Result;
                })
                .OrderByDescending(p => p?.TownHallLevel)
                .ThenBy(p => p?.Name)
                .ToArray();

            int nbPages = (int)
                Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
            bool isLastPage = nbPages < 2;

            // Filters for options
            availablePlayers = availablePlayers.Take(Settings.MAX_OPTION_PER_SELECT_MENU).ToArray();

            // Build select menu
            string customSelectId = isLastPage
                ? IdProvider.WAR_EDIT_SELECT_REMOVE_LAST_PLAYERS
                : IdProvider.WAR_EDIT_SELECT_REMOVE_PLAYERS;
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(customSelectId)
                .WithMinValues(1)
                .WithMaxValues(availablePlayers.Length);

            availablePlayers
                .ToList()
                .ForEach(p =>
                {
                    menuBuilder.AddOption(
                        p?.Name,
                        p?.Tag,
                        p?.Tag,
                        CustomEmojis.ParseTownHallLevel(p?.TownHallLevel ?? 0)
                    );
                });

            // Next button
            string customButtonId = isLastPage
                ? IdProvider.WAR_EDIT_BUTTON_REMOVE_LAST_NEXT_PLAYERS
                : IdProvider.WAR_EDIT_BUTTON_REMOVE_NEXT_PLAYERS;
            string buttonText = isLastPage
                ? interactionText.WarEditRemovePlayersEnd
                : interactionText.WarEditRemovePlayersNext;
            ButtonBuilder nextButtonBuilder = new ButtonBuilder()
                .WithLabel(buttonText)
                .WithDisabled(isLastPage)
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

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditRemovePlayersSelect(nbPages),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { eventId, "0" };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(
            IdProvider.WAR_EDIT_BUTTON_REMOVE_NEXT_PLAYERS,
            runMode: RunMode.Async
        )]
        public async Task EditRemoveSkipPlayers()
        {
            // Simply redirect to select component, exactly the same interaction
            await EditRemovePlayers(Array.Empty<string>());
        }

        [ComponentInteraction(
            IdProvider.WAR_EDIT_BUTTON_REMOVE_LAST_NEXT_PLAYERS,
            runMode: RunMode.Async
        )]
        public async Task EditRemoveSkipLastPlayers()
        {
            // Simply redirect to select component, exactly the same interaction
            await EditRemoveLastPlayers(Array.Empty<string>());
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_BUTTON_START_HOUR, runMode: RunMode.Async)]
        public async Task EditStartHour()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas[0];

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;
            CultureInfo cultureInfo = dbGuild.CultureInfo;

            NodaConverter converter = new();
            DateTimeOffset warDate = converter.ConvertDateTo(warEvent.Start, dbGuild.TimeZone);

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(
                IdProvider.WAR_EDIT_SELECT_START_HOUR
            );

            Enumerable
                .Range(0, 24)
                .Reverse()
                .ToList()
                .ForEach(
                    n =>
                        menuBuilder.AddOption(
                            warDate.Date.AddHours(n).ToString("t", cultureInfo),
                            n.ToString(),
                            emote: CustomEmojis.CocClock,
                            isDefault: n == warDate.Hour
                        )
                );

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditStartHourSelect,
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            storage.MessageDatas.TryAdd(message.Id, datas);
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
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .ToArray();

            // Gets all guild members
            IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 3)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string opponentTag = datas[0];
            string totalTime = datas[1];
            string warDate = datas[2];

            ulong competitionId = ulong.Parse(selections.First());
            Common.Models.Competition dbCompetition = competitions
                .AsParallel()
                .First(c => c.Guild == dbGuild && c.Id == competitionId);

            // Filters one more time
            Player[] availablePlayers = dbPlayers
                .Where(p =>
                {
                    SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

                    return user is not null
                        && user.IsAPlayer()
                        && user.Roles.Any(
                            r =>
                                r.Name.Contains(
                                    dbCompetition.Name,
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                        )
                        && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel;
                })
                .OrderByDescending(p => p.Account.TownHallLevel)
                .ThenBy(p => p.Account.Name)
                .ToArray();

            // Filters for options
            int nbPages = (int)
                Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
            bool isLastPage = 1 == nbPages;
            availablePlayers = availablePlayers.Take(Settings.MAX_OPTION_PER_SELECT_MENU).ToArray();

            // Build select menu
            string customSelectId = isLastPage
                ? IdProvider.WAR_ADD_SELECT_LAST_PLAYERS
                : IdProvider.WAR_ADD_SELECT_PLAYERS;

            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(customSelectId)
                .WithMinValues(1)
                .WithMaxValues(availablePlayers.Length);

            availablePlayers
                .ToList()
                .ForEach(p =>
                {
                    ClashOfClans.Models.Player cPlayer = p.Account;
                    menuBuilder.AddOption(
                        cPlayer.Name,
                        p.Tag,
                        p.Tag,
                        CustomEmojis.ParseTownHallLevel(cPlayer.TownHallLevel)
                    );
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

            IUserMessage message = await FollowupAsync(
                interactionText.WarAddCompetitionSelectPlayers(dbCompetition.Name, nbPages),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[]
            {
                opponentTag,
                totalTime.ToString(),
                warDate,
                dbCompetition.Id.ToString(),
                "0"
            };
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
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 3)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string opponentTag = datas[0];
            string totalTime = datas[1];

            int hours = int.Parse(selections.First());
            DateTimeOffset warDate = DateTimeOffset.Parse(datas[2]).AddHours(hours);

            CultureInfo cultureInfo = dbGuild.CultureInfo;

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(
                IdProvider.WAR_ADD_SELECT_MINUTES
            );

            Enumerable
                .Range(0, 4)
                .ToList()
                .ForEach(
                    n =>
                        menuBuilder.AddOption(
                            warDate.AddMinutes(n * 15).ToString("t", cultureInfo),
                            (n * 15).ToString(),
                            emote: CustomEmojis.CocClock
                        )
                );

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarAddHourSelectMinutes(warDate.ToString("t", cultureInfo)),
                components: componentBuilder.Build(),
                ephemeral: true
            );

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
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

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
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string opponentTag = datas[0];
            string totalTime = datas[1];

            int minutes = int.Parse(selections.First());
            DateTimeOffset warDate = DateTimeOffset.Parse(datas[2]).AddMinutes(minutes);

            CultureInfo cultureInfo = dbGuild.CultureInfo;

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(
                IdProvider.WAR_ADD_SELECT_COMPETITION
            );

            dbCompetitions
                .OrderBy(c => c.Name)
                .ToList()
                .ForEach(
                    c =>
                        menuBuilder.AddOption(
                            c.Name,
                            c.Id.ToString(),
                            emote: CustomEmojis.CocTrophy
                        )
                );

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarAddMinutesSelectCompetition(warDate.ToString("t", cultureInfo)),
                components: componentBuilder.Build(),
                ephemeral: true
            );

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
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 5)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

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
            Calendar dbCalendar = calendars.AsParallel().First(c => c.Guild == dbGuild);

            Common.Models.Competition dbCompetition = competitions
                .AsParallel()
                .First(c => c.Guild == dbGuild && c.Id == competitionId);

            ClashOfClans.Models.Clan? cOpponent = await ClashOfClansApi.Clans.GetByTagAsync(
                opponentTag
            );

            // Creates war match and inserts into calendar
            CalendarEvent calendarEvent =
                new(dbCompetition.Name, opponentTag, warDate, endDate, playersTag);
            Google.Apis.Calendar.v3.Data.Event? resultEvent =
                await GoogleCalendarApi.Events.InsertAsync(
                    calendarEvent,
                    dbGuild.TimeZone.AsAttribute().Zone.Id,
                    dbCalendar.Id
                );

            if (resultEvent is not null)
            {
                await FollowupAsync(
                    interactionText.WarAddLastPlayersMatchAdded(
                        warDate.ToString("d", cultureInfo),
                        cOpponent!.Name
                    ),
                    ephemeral: true
                );
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
            DbCompetitions competitions = Database.Context.Competitions;
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .ToArray();

            // Gets all guild members
            IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 5)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string opponentTag = datas![0];
            string totalTime = datas[1];
            string warDate = datas[2];
            string competitionId = datas[3];
            int currentPage = int.Parse(datas[4]) + 1;
            string[] playersTag = datas.Skip(5).Concat(selections).ToArray();

            // Recovers the competition
            Common.Models.Competition dbCompetition = competitions
                .AsParallel()
                .First(c => c.Guild == dbGuild && c.Id == ulong.Parse(competitionId));

            // Filters one more time
            Player[] availablePlayers = dbPlayers
                .Where(p =>
                {
                    SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

                    return user is not null
                        && user.IsAPlayer()
                        && user.Roles.Any(
                            r =>
                                r.Name.Contains(
                                    dbCompetition.Name,
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                        )
                        && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel;
                })
                .OrderByDescending(p => p.Account.TownHallLevel)
                .ThenBy(p => p.Account.Name)
                .ToArray();

            int nbPages = (int)
                Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
            bool isLastPage = currentPage + 1 == nbPages;

            // Filters for options
            availablePlayers = availablePlayers
                .Skip(currentPage * Settings.MAX_OPTION_PER_SELECT_MENU)
                .Take(Settings.MAX_OPTION_PER_SELECT_MENU)
                .ToArray();

            // Build select menu
            string customSelectId = isLastPage
                ? IdProvider.WAR_ADD_SELECT_LAST_PLAYERS
                : IdProvider.WAR_ADD_SELECT_PLAYERS;
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(customSelectId)
                .WithMinValues(1)
                .WithMaxValues(availablePlayers.Length);

            availablePlayers
                .ToList()
                .ForEach(p =>
                {
                    ClashOfClans.Models.Player cPlayer = p.Account;
                    menuBuilder.AddOption(
                        cPlayer.Name,
                        p.Tag,
                        p.Tag,
                        CustomEmojis.ParseTownHallLevel(cPlayer.TownHallLevel)
                    );
                });

            // Next button
            string customButtonId = isLastPage
                ? IdProvider.WAR_ADD_BUTTON_LAST_NEXT_PLAYERS
                : IdProvider.WAR_ADD_BUTTON_NEXT_PLAYERS;
            string buttonText = isLastPage
                ? interactionText.WarAddPlayersEnd
                : interactionText.WarAddCompetitionNextPlayers;
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

            IUserMessage message = await FollowupAsync(
                interactionText.WarAddPlayersSelectPlayers(
                    currentPage + 1,
                    nbPages,
                    playersTag.Length
                ),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[]
            {
                opponentTag,
                totalTime.ToString(),
                warDate,
                competitionId,
                currentPage.ToString()
            }
                .Concat(playersTag)
                .ToArray();
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
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Calendar dbCalendar = calendars.AsParallel().First(c => c.Guild == dbGuild);

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

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_ADD_LAST_PLAYERS, runMode: RunMode.Async)]
        public async Task EditAddLastPlayers(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            DbCompetitions competitions = Database.Context.Competitions;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Calendar dbCalendar = calendars.AsParallel().First(c => c.Guild == dbGuild);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 2)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas![0];
            int currentPage = int.Parse(datas[1]);
            string[] playersTag = datas.Skip(2).Concat(selections).ToArray();

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Updates war match and saves into calendar
            warEvent.Players = warEvent.Players.Concat(playersTag).ToArray();

            if (!await GoogleCalendarApi.Events.UpdateAsync(warEvent, dbCalendar.Id))
            {
                await FollowupAsync(generalResponses.GoogleCannotUpdateEvent, ephemeral: true);

                return;
            }

            await FollowupAsync(
                interactionText.WarEditAddPlayersUpdated(playersTag.Length),
                ephemeral: true
            );
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_ADD_PLAYERS, runMode: RunMode.Async)]
        public async Task EditAddPlayers(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Filters for guild
            Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .ToArray();

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 2)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas![0];
            int currentPage = int.Parse(datas[1]) + 1;
            string[] playersTag = datas.Skip(2).Concat(selections).ToArray();

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Gets all guild members
            IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

            // Filters one more time
            Player[] availablePlayers = dbPlayers
                .Where(p =>
                {
                    SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

                    return user is not null
                        && user.IsAPlayer()
                        && user.Roles.Any(
                            r =>
                                r.Name.Contains(
                                    warEvent.CompetitionName,
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                        )
                        && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel
                        && !warEvent.Players.Contains(
                            p.Tag,
                            StringComparer.InvariantCultureIgnoreCase
                        );
                })
                .OrderByDescending(p => p.Account.TownHallLevel)
                .ThenBy(p => p.Account.Name)
                .ToArray();

            int nbPages = (int)
                Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
            bool isLastPage = currentPage + 1 == nbPages;

            // Filters for options
            availablePlayers = availablePlayers
                .Skip(currentPage * Settings.MAX_OPTION_PER_SELECT_MENU)
                .Take(Settings.MAX_OPTION_PER_SELECT_MENU)
                .ToArray();

            // Build select menu
            string customSelectId = isLastPage
                ? IdProvider.WAR_EDIT_SELECT_ADD_LAST_PLAYERS
                : IdProvider.WAR_EDIT_SELECT_ADD_PLAYERS;
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(customSelectId)
                .WithMinValues(1)
                .WithMaxValues(availablePlayers.Length);

            availablePlayers
                .ToList()
                .ForEach(p =>
                {
                    ClashOfClans.Models.Player cPlayer = p.Account;
                    menuBuilder.AddOption(
                        cPlayer.Name,
                        p.Tag,
                        p.Tag,
                        CustomEmojis.ParseTownHallLevel(cPlayer.TownHallLevel)
                    );
                });

            // Next button
            string customButtonId = isLastPage
                ? IdProvider.WAR_EDIT_BUTTON_ADD_LAST_NEXT_PLAYERS
                : IdProvider.WAR_EDIT_BUTTON_ADD_NEXT_PLAYERS;
            string buttonText = isLastPage
                ? interactionText.WarEditAddPlayersEnd
                : interactionText.WarEditAddPlayersNext;
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

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditAddPlayersSelect(
                    currentPage + 1,
                    nbPages,
                    playersTag.Length
                ),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { eventId, currentPage.ToString() }.Concat(playersTag).ToArray();
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_CHOOSE_UPDATE, runMode: RunMode.Async)]
        public async Task EditChooseUpdate(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets data
            string eventId = selections.First();

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Calendar dbCalendar = calendars.AsParallel().First(c => c.Guild == dbGuild);

            Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .ToArray();

            // Gets event informations
            CalendarEvent clashEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Gets all guild members
            IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

            // Filters one more time
            Player[] availablePlayers = dbPlayers
                .Where(p =>
                {
                    SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

                    return user is not null
                        && user.IsAPlayer()
                        && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel
                        && !clashEvent.Players.Contains(p.Tag);
                })
                .OrderByDescending(p => p.Account.TownHallLevel)
                .ThenBy(p => p.Account.Name)
                .ToArray();

            // Gets command responses
            IAdmin adminResponses = dbGuild.AdminText;
            IManager commandText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Change opponent
            ButtonBuilder opponentButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.WarEditOpponent)
                .WithStyle(ButtonStyle.Secondary)
                .WithCustomId(IdProvider.WAR_EDIT_BUTTON_OPPONENT);

            // Change format
            ButtonBuilder formatButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.WarEditFormat)
                .WithStyle(ButtonStyle.Secondary)
                .WithCustomId(IdProvider.WAR_EDIT_BUTTON_FORMAT);

            // Change day
            ButtonBuilder dayButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.WarEditDay)
                .WithStyle(ButtonStyle.Secondary)
                .WithCustomId(IdProvider.WAR_EDIT_BUTTON_DAY);

            // Change hour
            ButtonBuilder timeButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.WarEditStartHour)
                .WithStyle(ButtonStyle.Secondary)
                .WithCustomId(IdProvider.WAR_EDIT_BUTTON_START_HOUR);

            // Remove players
            ButtonBuilder removePlayerButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.WarEditRemovePlayer)
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(!clashEvent.Players.Any())
                .WithCustomId(IdProvider.WAR_EDIT_BUTTON_REMOVE_PLAYER);

            // Add players
            ButtonBuilder addPlayerButtonBuilder = new ButtonBuilder()
                .WithLabel(commandText.WarEditAddPlayer)
                .WithStyle(ButtonStyle.Success)
                .WithDisabled(!availablePlayers.Any())
                .WithCustomId(IdProvider.WAR_EDIT_BUTTON_ADD_PLAYER);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithButton(opponentButtonBuilder)
                .WithButton(formatButtonBuilder)
                .WithButton(dayButtonBuilder, 1)
                .WithButton(timeButtonBuilder, 1)
                .WithButton(removePlayerButtonBuilder, 2)
                .WithButton(addPlayerButtonBuilder, 2)
                .WithButton(cancelButtonBuilder, 3);

            IUserMessage message = await FollowupAsync(
                commandText.WarEditChooseEdition(clashEvent.OpponentClan.Name),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Registers informations into storage
            ComponentStorage storage = ComponentStorage.GetInstance();

            string[] datas = new[] { eventId };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_DAY, runMode: RunMode.Async)]
        public async Task EditDay(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas[0];
            DateTimeOffset warDate = DateTimeOffset.Parse(selections.First());

            // Gets event
            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Gets format to update end date too
            NodaConverter converter = new();

            DateTimeOffset start = converter.ConvertDateTo(warEvent.Start, dbGuild.TimeZone);
            TimeSpan format = warEvent.End - warEvent.Start;

            // Updates and saves
            warEvent.Start = warDate.AddHours(start.Hour).AddMinutes(start.Minute);
            warEvent.End = warEvent.Start.AddMinutes(format.TotalMinutes);

            if (!await GoogleCalendarApi.Events.UpdateAsync(warEvent, dbCalendar.Id))
            {
                await FollowupAsync(generalResponses.GoogleCannotUpdateEvent, ephemeral: true);

                return;
            }

            await FollowupAsync(
                interactionText.WarEditDayChanged(warDate.ToString("d", dbGuild.CultureInfo)),
                ephemeral: true
            );
        }

        [ComponentInteraction(
            IdProvider.WAR_EDIT_SELECT_FORMAT_PREPARATION,
            runMode: RunMode.Async
        )]
        public async Task EditFormatPreparation(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await RespondAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

                return;
            }

            // Build options
            List<SelectMenuOptionBuilder> options =
                new()
                {
                    new SelectMenuOptionBuilder($"15 {generalResponses.Minutes}", "15"),
                    new SelectMenuOptionBuilder($"30 {generalResponses.Minutes}", "30"),
                    new SelectMenuOptionBuilder($"45 {generalResponses.Minutes}", "45"),
                    new SelectMenuOptionBuilder($"1 {generalResponses.Hour}", "60"),
                    new SelectMenuOptionBuilder($"2 {generalResponses.Hours}", "120"),
                    new SelectMenuOptionBuilder($"4 {generalResponses.Hours}", "240"),
                    new SelectMenuOptionBuilder($"6 {generalResponses.Hours}", "360"),
                    new SelectMenuOptionBuilder($"8 {generalResponses.Hours}", "480"),
                    new SelectMenuOptionBuilder($"12 {generalResponses.Hours}", "720"),
                    new SelectMenuOptionBuilder($"16 {generalResponses.Hours}", "960"),
                    new SelectMenuOptionBuilder($"20 {generalResponses.Hours}", "1200"),
                    new SelectMenuOptionBuilder($"1 {generalResponses.Day}", "1440"),
                };

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.WAR_EDIT_SELECT_FORMAT_WAR)
                .WithOptions(options);

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditFormatPreparation,
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { datas[0], selections.First() };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_FORMAT_WAR, runMode: RunMode.Async)]
        public async Task EditFormatWar(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 2)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas[0];

            int preparationTime = int.Parse(datas[1]);
            int warTime = int.Parse(selections.First());

            TimeSpan format = TimeSpan.FromMinutes(preparationTime + warTime);
            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Updates and saves
            warEvent.End = warEvent.Start.AddMinutes(format.TotalMinutes);

            if (!await GoogleCalendarApi.Events.UpdateAsync(warEvent, dbCalendar.Id))
            {
                await FollowupAsync(generalResponses.GoogleCannotUpdateEvent, ephemeral: true);

                return;
            }

            await FollowupAsync(interactionText.WarEditFormatChanged, ephemeral: true);
        }

        [ComponentInteraction(
            IdProvider.WAR_EDIT_SELECT_REMOVE_LAST_PLAYERS,
            runMode: RunMode.Async
        )]
        public async Task EditRemoveLastPlayers(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            DbCalendars calendars = Database.Context.Calendars;
            DbCompetitions competitions = Database.Context.Competitions;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Calendar dbCalendar = calendars.AsParallel().First(c => c.Guild == dbGuild);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 2)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas![0];
            int currentPage = int.Parse(datas[1]);
            string[] playersTag = datas.Skip(2).Concat(selections).ToArray();

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Updates war match and saves into calendar
            warEvent.Players = warEvent.Players.Except(playersTag).ToArray();

            if (!await GoogleCalendarApi.Events.UpdateAsync(warEvent, dbCalendar.Id))
            {
                await FollowupAsync(generalResponses.GoogleCannotUpdateEvent, ephemeral: true);

                return;
            }

            await FollowupAsync(
                interactionText.WarEditRemovePlayersUpdated(playersTag.Length),
                ephemeral: true
            );
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_REMOVE_PLAYERS, runMode: RunMode.Async)]
        public async Task EditRemovePlayers(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length < 2)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas![0];
            int currentPage = int.Parse(datas[1]) + 1;
            string[] playersTag = datas.Skip(2).Concat(selections).ToArray();

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Filters one more time
            ClashOfClans.Models.Player?[] availablePlayers = warEvent
                .Players.Select(p =>
                {
                    Task<ClashOfClans.Models.Player?> playerTask =
                        ClashOfClansApi.Players.GetByTagAsync(p);
                    playerTask.Wait();

                    return playerTask.Result;
                })
                .OrderByDescending(p => p?.TownHallLevel)
                .ThenBy(p => p?.Name)
                .ToArray();

            int nbPages = (int)
                Math.Ceiling((double)availablePlayers.Length / Settings.MAX_OPTION_PER_SELECT_MENU);
            bool isLastPage = currentPage + 1 == nbPages;

            // Filters for options
            availablePlayers = availablePlayers
                .Skip(currentPage * Settings.MAX_OPTION_PER_SELECT_MENU)
                .Take(Settings.MAX_OPTION_PER_SELECT_MENU)
                .ToArray();

            // Build select menu
            string customSelectId = isLastPage
                ? IdProvider.WAR_EDIT_SELECT_REMOVE_LAST_PLAYERS
                : IdProvider.WAR_EDIT_SELECT_REMOVE_PLAYERS;
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(customSelectId)
                .WithMinValues(1)
                .WithMaxValues(availablePlayers.Length);

            availablePlayers
                .ToList()
                .ForEach(p =>
                {
                    menuBuilder.AddOption(
                        p?.Name,
                        p?.Tag,
                        p?.Tag,
                        CustomEmojis.ParseTownHallLevel(p?.TownHallLevel ?? 0)
                    );
                });

            // Next button
            string customButtonId = isLastPage
                ? IdProvider.WAR_EDIT_BUTTON_REMOVE_LAST_NEXT_PLAYERS
                : IdProvider.WAR_EDIT_BUTTON_REMOVE_NEXT_PLAYERS;
            string buttonText = isLastPage
                ? interactionText.WarEditRemovePlayersEnd
                : interactionText.WarEditRemovePlayersNext;
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

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditRemovePlayersSelect(
                    currentPage + 1,
                    nbPages,
                    playersTag.Length
                ),
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { eventId, currentPage.ToString() }.Concat(playersTag).ToArray();
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_START_HOUR, runMode: RunMode.Async)]
        public async Task EditStartHour(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 1)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas[0];
            int hours = int.Parse(selections.First());

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;
            CultureInfo cultureInfo = dbGuild.CultureInfo;

            NodaConverter converter = new();
            DateTimeOffset warDate = converter.ConvertDateTo(warEvent.Start, dbGuild.TimeZone);

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(
                IdProvider.WAR_EDIT_SELECT_START_MINUTE
            );

            Enumerable
                .Range(0, 4)
                .ToList()
                .ForEach(
                    n =>
                        menuBuilder.AddOption(
                            warDate
                                .Date.AddHours(hours)
                                .AddMinutes(n * 15)
                                .ToString("t", cultureInfo),
                            (n * 15).ToString(),
                            emote: CustomEmojis.CocClock
                        )
                );

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(
                interactionText.WarEditStartHourSelectMinutes,
                components: componentBuilder.Build(),
                ephemeral: true
            );

            // Inserts new datas
            datas = new[] { eventId, hours.ToString() };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_EDIT_SELECT_START_MINUTE, runMode: RunMode.Async)]
        public async Task EditStartMinute(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            Guild dbGuild = Database.Context.Guilds.First(g => g.Id == Context.Guild.Id);

            Calendar dbCalendar = Database.Context.Calendars.First(c => c.Guild == dbGuild);

            // Gets interaction text
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 2)
            {
                await FollowupAsync(
                    generalResponses.FailToGetStorageComponentData,
                    ephemeral: true
                );

                return;
            }

            // Recovers datas
            string eventId = datas[0];

            int hours = int.Parse(datas[1]);
            int minutes = int.Parse(selections.First());

            CalendarEvent warEvent = (
                await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId)
            )!;

            // Updates and saves
            NodaConverter converter = new();
            TimeSpan format = warEvent.End - warEvent.Start;

            DateTimeOffset warDate =
                new(
                    converter.ConvertDateTo(warEvent.Start, dbGuild.TimeZone).Date,
                    dbGuild.Now.Offset
                );

            warEvent.Start = warDate.AddHours(hours).AddMinutes(minutes);
            warEvent.End = warEvent.Start.AddMinutes(format.TotalMinutes);

            if (!await GoogleCalendarApi.Events.UpdateAsync(warEvent, dbCalendar.Id))
            {
                await FollowupAsync(generalResponses.GoogleCannotUpdateEvent, ephemeral: true);

                return;
            }

            await FollowupAsync(
                interactionText.WarEditStartHourUpdated(
                    warEvent.Start.ToString("HH:mm", dbGuild.CultureInfo)
                ),
                ephemeral: true
            );
        }
    }
}
