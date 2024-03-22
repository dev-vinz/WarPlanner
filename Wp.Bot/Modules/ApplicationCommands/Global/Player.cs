using Discord;
using Discord.Interactions;
using Wp.Api;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Discord;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Global
{
    public class Player : InteractionModuleBase<SocketInteractionContext>
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

        public Player(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [SlashCommand("claim", "Link a COC account to your Discord profile", runMode: RunMode.Async)]
        public async Task ClaimAccount()
        {
            // Gets guild and command text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IGlobal commandText = dbGuild.GlobalText;

            // Creates modal
            ModalBuilder modalBuilder = new ModalBuilder()
                .WithTitle(commandText.PlayerClaimTitle)
                .WithCustomId(PlayerClaimModal.ID)
                .AddTextInput(commandText.PlayerClaimTagField, PlayerClaimModal.TAG_ID, placeholder: "#ABCD1234")
                .AddTextInput(commandText.PlayerClaimTokenField, PlayerClaimModal.TOKEN_ID, placeholder: "xh35nbdr", minLength: PlayerClaimModal.MIN_TOKEN_LENGTH, maxLength: PlayerClaimModal.MAX_TOKEN_LENGTH);

            await RespondWithModalAsync(modalBuilder.Build());
        }

        [SlashCommand("accounts", "Gets all your registered accounts", runMode: RunMode.Async)]
        public async Task GetAccounts()
        {
            await DeferAsync(true);

            // Loads database infos
            DbPlayers players = Database.Context.Players;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild and user
            Common.Models.Player[] dbPlayers = players
                .AsParallel()
                .Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
                .Where(p => p.Id == Context.User.Id)
                .ToArray();

            // Gets responses
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;
            IGlobal commandText = dbGuild.GlobalText;

            if (!dbPlayers.Any())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.PlayerAccountsNoAccounts);

                return;
            }

            // Checks API
            if (!await ClashOfClansApi.TryAccessApiAsync())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

                return;
            }

            // Build an embed
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(commandText.PlayerAccountsEmbedTitle)
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .WithDescription($"{CustomEmojis.WarPlanner} | {commandText.PlayerAccountsEmbedDescription}")
                .WithRandomColor()
                .WithFooter($"{dbGuild.Now.Year} © {Context.Client.CurrentUser.Username}", Context.Client.CurrentUser.GetAvatarUrl());

            Dictionary<int, List<Common.Models.Player>> dictAccounts = dbPlayers
                .OrderByDescending(p => p.Account.TownHallLevel)
                .GroupBy(p => p.Account.TownHallLevel)
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.Account.Name).ToList());

            foreach (KeyValuePair<int, List<Common.Models.Player>> kvp in dictAccounts)
            {
                int thLevel = kvp.Key;
                List<Common.Models.Player> accounts = kvp.Value;

                embedBuilder.AddField($"{CustomEmojis.ParseTownHallLevel(thLevel)} {generalResponses.TownHallShortcut} {thLevel}", $"× {string.Join("\n× ", accounts.Select(a => a.Account.Name))}", true);
            }

            await ModifyOriginalResponseAsync(msg => msg.Embed = embedBuilder.Build());
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
