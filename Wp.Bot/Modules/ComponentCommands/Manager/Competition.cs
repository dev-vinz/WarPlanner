using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Wp.Api;
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
            if (!TryDecodeButtonInteraction(IdProvider.COMPETITION_ADD_BUTTON_NO_SECOND_CLAN, out ButtonSerializer buttonSerializer)) return;

            await DeferAsync();

            // Get SocketMessageComponent and original message
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
            if (!storage.ComponentDatas.TryGetValue(msg.Id, out string[]? datas) && datas?.Length != 2)
            {
                await FollowupAsync(generalResponses.FailToGetStorageComponentData);

                return;
            }

            // Remove datas from storage
            storage.ComponentDatas.Remove(msg.Id);

            // Guild roles
            IReadOnlyCollection<IRole> guildRoles = roles
                .AsParallel()
                .Where(r => r.Guild == dbGuild && r.Type == RoleType.PLAYER)
                .Select(r => Context.Guild.GetRole(r.Id))
                .ToArray();

            // Recovers datas
            string competitionName = datas[0];
            string clanTag = datas[1];

            // Create environment
            if (!TryCreateEnvironment(competitionName, guildRoles, out ulong? categoryId, out ulong? resultId, out IRole? refRole))
            {
                await FollowupAsync(interactionText.CouldntCreateCompetitionEnvironment);

                return;
            }

            // Register competition
            Common.Models.Competition dbCompetition = new(dbGuild, categoryId!.Value, resultId!.Value, competitionName, clanTag);
            competitions.Add(dbCompetition);

            await FollowupAsync(interactionText.CompetitionAdded(competitionName, dbCompetition.MainClan.Name));
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.COMPETITION_ADD_SELECT_MAIN_CLAN, runMode: RunMode.Async)]
        public async Task AddMainClanSelect(string[] selections)
        {
            if (!TryDecodeSelectInteraction(selections, IdProvider.COMPETITION_ADD_SELECT_MAIN_CLAN, out SelectSerializer selectSerializer, out SelectOptionSerializer[] optionsSerializer)) return;

            await DeferAsync();

            SelectOptionSerializer option = optionsSerializer.First();

            // Loads databases infos
            DbClans clans = Database.Context.Clans;
            DbCompetitions competitions = Database.Context.Competitions;
            DbRoles roles = Database.Context.Roles;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Common.Models.Clan[] dbClans = clans.AsParallel().Where(c => c.Guild == dbGuild).ToArray();
            Common.Models.Competition[] dbCompetitions = competitions.AsParallel().Where(c => c.Guild == dbGuild).ToArray();

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            string competitionName = option.Datas[0];

            // Checks Clash Of Clans API
            if (!await ClashOfClansApi.TryAccessApiAsync())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

                return;
            }

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.COMPETITION_ADD_SELECT_SECOND_CLAN);

            dbClans
                .AsParallel()
                .ForAll(c =>
                {
                    SelectOptionSerializer optionSerializer = new(dbGuild.Id, Context.User.Id, c.Tag, competitionName, option.Value);
                    menuBuilder.AddOption(c.Profile.Name, optionSerializer.ToString(), c.Tag);
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
            IUserMessage message = await FollowupAsync(interactionText.ChooseCompetitionSecondClan, components: componentBuilder.Build());

            // Registers informations into storage
            ComponentStorage storage = ComponentStorage.GetInstance();

            string[] datas = new[] { competitionName, option.Value };
            storage.ComponentDatas.Add(message.Id, datas);

            message.DeleteAllComponentsAfterButtonClick(cancelButtonBuilder.CustomId, Context.User.Id);
            message.DisableButtonAfterClick(noneButtonBuilder.CustomId, Context.User.Id, disableAllComponents: true);
            message.DisableSelectAfterSelection(menuBuilder.CustomId, Context.User.Id, removeButtons: true);
        }

        [ComponentInteraction(IdProvider.COMPETITION_ADD_SELECT_SECOND_CLAN, runMode: RunMode.Async)]
        public async Task AddSecondClanSelect(string[] selections)
        {
            if (!TryDecodeSelectInteraction(selections, IdProvider.COMPETITION_ADD_SELECT_SECOND_CLAN, out SelectSerializer selectSerializer, out SelectOptionSerializer[] optionsSerializer)) return;

            await DeferAsync();

            SelectOptionSerializer option = optionsSerializer.First();

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

            // Recovers datas
            string competitionName = option.Datas[0];
            string clanTag = option.Datas[1];
            string secondTag = option.Value;

            // Create environment
            if (!TryCreateEnvironment(competitionName, guildRoles, out ulong? categoryId, out ulong? resultId, out IRole? refRole))
            {
                await FollowupAsync(interactionText.CouldntCreateCompetitionEnvironment);

                return;
            }

            // Register competition
            Common.Models.Competition dbCompetition = new(dbGuild, categoryId!.Value, resultId!.Value, competitionName, clanTag)
            {
                SecondTag = secondTag,
            };
            competitions.Add(dbCompetition);

            await FollowupAsync(interactionText.CompetitionAdded(competitionName, dbCompetition.MainClan.Name, dbCompetition.SecondClan?.Name));
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

                refRole = guild.CreateRoleAsync(interactionText.CompetitionEnvironmentReferentRoleName(competitionName), GuildPermissions.None, isMentionable: true).Result;
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
                category.AddPermissionOverwriteAsync(refRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
                category.AddPermissionOverwriteAsync(tournamentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
                category.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)).Wait();

                informations.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
                informations.AddPermissionOverwriteAsync(refRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
                informations.AddPermissionOverwriteAsync(tournamentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny)).Wait();
                informations.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)).Wait();

                result.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
                result.AddPermissionOverwriteAsync(refRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow, sendMessages: PermValue.Allow)).Wait();
                result.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny)).Wait();

                voice.AddPermissionOverwriteAsync(botMember, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow)).Wait();
                voice.AddPermissionOverwriteAsync(refRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, useVoiceActivation: PermValue.Allow, speak: PermValue.Allow)).Wait();
                voice.AddPermissionOverwriteAsync(tournamentRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, useVoiceActivation: PermValue.Allow, speak: PermValue.Allow)).Wait();
                voice.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)).Wait();

                /* * * * * * * * * * * * * * * *\
                |*      RESULT PERMISSION      *|
                \* * * * * * * * * * * * * * * */

                playerRoles
                    .AsParallel()
                    .ForAll(async role => await result.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny)));

                /* * * * * * * * * * * * * * * *\
                |*          RETURN IDS         *|
                \* * * * * * * * * * * * * * * */

                categoryId = category.Id;
                resultId = result.Id;

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

        private bool TryDecodeButtonInteraction(string buttonId, out ButtonSerializer buttonSerializer)
        {
            // Get SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            buttonSerializer = new(Context.User.Id, msg.Id, buttonId);

            // Gets guild and interaction text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IManager interactionText = dbGuild.ManagerText;

            // Encodes key
            string key = buttonSerializer.Encode();

            // Gets user id associated
            ComponentStorage componentStorage = ComponentStorage.GetInstance();
            ulong userId = componentStorage.Buttons.GetValueOrDefault(key);

            // Checks if user is elligible for interaction
            if (Context.User.Id != userId)
            {
                Task response = RespondAsync(interactionText.UserNotAllowedToSelect, ephemeral: true);
                response.Wait();

                return false;
            }

            componentStorage.Buttons.Remove(key);

            return true;
        }

        private bool TryDecodeSelectInteraction(string[] selections, string selectId, out SelectSerializer selectSerializer, out SelectOptionSerializer[] optionsSerializer)
        {
            // Get SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Decodes all options
            List<SelectOptionSerializer?> options = new();

            selections
                .AsParallel()
                .ForAll(s => options.Add(SelectOptionSerializer.Decode(s)));

            selectSerializer = new(Context.User.Id, msg.Id, selectId);
            optionsSerializer = options
                .AsParallel()
                .Where(o => o != null)
                .ToArray()!;

            // Gets guild and interaction text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IManager interactionText = dbGuild.ManagerText;

            // Checks if there's options in the interaction
            if (optionsSerializer.Length < 1)
            {
                Task response = RespondAsync(interactionText.SelectDontContainsOption, ephemeral: true);
                response.Wait();

                return false;
            }

            // Checks if user is elligible for interaction
            if (Context.User.Id != optionsSerializer.First().UserId)
            {
                Task response = RespondAsync(interactionText.UserNotAllowedToSelect, ephemeral: true);
                response.Wait();

                return false;
            }

            ComponentStorage componentStorage = ComponentStorage.GetInstance();

            // Encodes key
            string key = selectSerializer.Encode();

            componentStorage.Selects.Remove(key);

            return true;
        }
    }
}
