using Discord.Interactions;
using Discord.WebSocket;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands
{
    public class GlobalButtons : InteractionModuleBase<SocketInteractionContext>
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

        public GlobalButtons(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.GLOBAL_CANCEL_BUTTON, runMode: RunMode.Async)]
        public async Task CancelButton()
        {
            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Gets guild and interaction text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Remove any storage component data
            ComponentStorage storage = ComponentStorage.GetInstance();
            storage.MessageDatas.TryRemove(msg.Id, out string[] _);

            await FollowupAsync(generalResponses.ActionCanceledByButton, ephemeral: true);
        }
    }
}
