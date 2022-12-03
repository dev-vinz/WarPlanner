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
            //if (!TryDecodeButtonInteraction(IdProvider.GLOBAL_CANCEL_BUTTON, out ButtonSerializer buttonSerializer)) return;

            //await DeferAsync();

            //// Gets guild and interaction text
            //Guild dbGuild = Database.Context
            //    .Guilds
            //    .First(g => g.Id == Context.Guild.Id);

            //// Get SocketMessageComponent and original message
            //SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            //SocketUserMessage msg = socket.Message;

            //// Remove any storage component data
            //ComponentStorage storage = ComponentStorage.GetInstance();
            //storage.MessageDatas.TryRemove(msg.Id, out string[] _);

            //IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            //await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ActionCanceledByButton);

            // Get SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Gets original user
            //ulong userId = msg.Interaction.User.Id;

            // Gets guild and interaction text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Checks if user is elligible for interaction
            //if (Context.User.Id != userId)
            //{
            //    await RespondAsync(interactionText.UserNotAllowedToInteract, ephemeral: true);

            //    return;
            //}

            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Remove any storage component data
            ComponentStorage storage = ComponentStorage.GetInstance();
            storage.MessageDatas.TryRemove(msg.Id, out string[] _);

            await FollowupAsync(generalResponses.ActionCanceledByButton, ephemeral: true);
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
                Task response = RespondAsync(interactionText.UserNotAllowedToInteract, ephemeral: true);
                response.Wait();

                return false;
            }

            componentStorage.Buttons.TryRemove(key, out ulong _);

            return true;
        }
    }
}
