using ClashOfClans.Models;
using Discord;
using Discord.Interactions;
using Wp.Api;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Language;

namespace Wp.Bot.Modules.ModalCommands.Global
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

        [ModalInteraction(PlayerClaimModal.ID, runMode: RunMode.Async)]
        public async Task ClaimAccount(PlayerClaimModal modal)
        {
            await DeferAsync();

            // Loads databases infos
            DbGuilds guilds = Database.Context.Guilds;
            DbPlayers players = Database.Context.Players;

            // Gets current guild
            Guild dbGuild = guilds
                .First(g => g.Id == Context.Guild.Id);

            // Gets general responses
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            ClashOfClans.Models.Player? cPlayer = await ClashOfClansApi.Players.GetByTagAsync(modal.Tag);

            if (cPlayer is null)
            {
                await FollowupAsync(generalResponses.ClashOfClansError);

                return;
            }

            // Gets modal responses
            IGlobal commandText = dbGuild.GlobalText;

            VerifyTokenResponse? verifyToken = await ClashOfClansApi.Players.VerifyTokenAsync(cPlayer.Tag, modal.Token);

            if (verifyToken?.Status != Status.Ok)
            {
                await FollowupAsync(commandText.TokenInvalid(cPlayer.Name));

                return;
            }

            // Gets any account
            IEnumerable<Common.Models.Player?> anyPlayers = players
                .Where(p => p.Tag == cPlayer.Tag);

            // Checks if it's a global account
            if (anyPlayers.Any() && anyPlayers.First()?.Guild?.Id == Configurations.DEV_GUILD_ID)
            {
                ButtonBuilder supportButton = new ButtonBuilder()
                    .WithUrl(Utilities.SUPPORT_GUILD_INVITATION)
                    .WithLabel(generalResponses.SupportServer)
                    .WithStyle(ButtonStyle.Link)
                    .WithDisabled(false)
                    .WithEmote(new Emoji("⚙️"));

                ActionRowBuilder rowBuilder = new ActionRowBuilder()
                    .WithButton(supportButton);

                ComponentBuilder componentBuilder = new ComponentBuilder()
                    .AddRow(rowBuilder);

                await FollowupAsync(commandText.AccountAlreadyClaimed(cPlayer.Name), components: componentBuilder.Build());

                return;
            }

            // Remove local accounts
            anyPlayers
                .AsParallel()
                .ForAll(p => players.Remove(p!));

            Guild devGuild = guilds
                .First(g => g.Id == Configurations.DEV_GUILD_ID);

            Common.Models.Player dbPlayer = new(devGuild, Context.User.Id, cPlayer.Tag);

            // Register new global account
            players.Add(dbPlayer);

            await FollowupAsync(commandText.AccountClaimed(cPlayer.Name));
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
