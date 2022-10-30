using ClashOfClans.Models;
using Discord;
using Discord.Interactions;
using Wp.Api;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Bot.Services.Languages;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Database.Services.Extensions;
using Wp.Database.Settings;

namespace Wp.Bot.Modules.ModalCommands.Player
{
    public class Player : ModalCommandModel
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly CommandHandler handler;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



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

        [ModalInteraction(ClaimModal.ID)]
        public async Task ClaimAccount(ClaimModal modal)
        {
            await RespondAsync("Loading...", ephemeral: true); // TODO : Change to DeferAsync... but seem's to doesn't work...
            //await DeferAsync();

            // Loads databases infos
            DbGuilds guilds = Database.Context.Guilds;
            DbPlayers players = Database.Context.Players;

            // Gets current guild
            Guild dbGuild = guilds
                .First(g => g.Id == Context.Guild.Id);

            // Gets global responses
            IGlobalResponse globalResponses = dbGuild
                .Language
                .GetGlobalResponse();

            ClashOfClans.Models.Player? cPlayer = await ClashOfClansApi.Players.GetByTagAsync(modal.Tag);

            if (cPlayer is null)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = globalResponses.ClashOfClansError);

                return;
            }

            // Gets commands responses
            IPlayer commandText = GetPlayerModalText();

            VerifyTokenResponse? verifyToken = await ClashOfClansApi.Players.VerifyTokenAsync(cPlayer.Tag, modal.Token);

            if (verifyToken?.Status != Status.Ok)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.TokenInvalid(cPlayer.Name));

                return;
            }

            // Gets any account
            IEnumerable<Common.Models.Player?> anyPlayers = players
                .Where(p => p.Tag == cPlayer.Tag);

            // Checks if it's a global account
            if (anyPlayers.Any() && anyPlayers.First()?.Guild?.Id == Configurations.DEV_GUILD_ID)
            {
                ButtonBuilder supportButton = new ButtonBuilder()
                    .WithUrl(Configurations.SUPPORT_GUILD_INVITATION)
                    .WithLabel(globalResponses.SupportServer)
                    .WithStyle(ButtonStyle.Link)
                    .WithDisabled(false)
                    .WithEmote(new Emoji("⚙️"));

                ActionRowBuilder rowBuilder = new ActionRowBuilder()
                    .WithButton(supportButton);

                ComponentBuilder componentBuilder = new ComponentBuilder()
                    .AddRow(rowBuilder);

                await ModifyOriginalResponseAsync(msg =>
                {
                    msg.Content = commandText.AccountAlreadyClaimed(cPlayer.Name);
                    msg.Components = new(componentBuilder.Build());
                });

                return;
            }

            // Remove local accounts
            anyPlayers.ForEach(p => players.Remove(p!));

            Guild devGuild = guilds
                .First(g => g.Id == Configurations.DEV_GUILD_ID);

            Common.Models.Player dbPlayer = new(devGuild, Context.User.Id, cPlayer.Tag);

            // Register new global account
            players.Add(dbPlayer);

            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = commandText.AccountClaimed(cPlayer.Name);
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
