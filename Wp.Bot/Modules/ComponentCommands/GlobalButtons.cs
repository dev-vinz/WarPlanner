using Discord.Interactions;
using Discord.WebSocket;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Discord.ComponentInteraction;
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
            if (!TryDecodeButtonInteraction(IdProvider.GLOBAL_CANCEL_BUTTON, out ButtonSerializer buttonSerializer)) return;

            await DeferAsync(true);

            // Gets guild and interaction text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Get SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Remove any storage component data
            ComponentStorage storage = ComponentStorage.GetInstance();
            storage.ComponentDatas.Remove(msg.Id);

            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ActionCanceledByButton);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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
    }
}
