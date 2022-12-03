using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using Wp.Bot.Services.Logger;

namespace Wp.Bot.Services
{
    public class CommandHandler
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly DiscordSocketClient client;
        private readonly InteractionService commands;
        private readonly IServiceProvider services;
        private readonly CommandLogger logger;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public CommandLogger Logger { get => logger; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public CommandHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            // Inputs
            {
                this.client = client;
                this.commands = commands;
                this.services = services;
            }

            // Tools
            {
                logger = new CommandLogger(client);
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public async Task InitializeAsync()
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            // Process the InteractionCreated payloads to execute Interactions commands
            client.InteractionCreated += HandleInteraction;

            // Process the command execution results
            commands.SlashCommandExecuted += SlashCommandExecuted;
            commands.ModalCommandExecuted += ModalCommandExecuted;
            commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private async Task ComponentCommandExecuted(ComponentCommandInfo info, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                await logger.LogComponentCommandAsync(info, ctx, result);
            }
        }

        private async Task ModalCommandExecuted(ModalCommandInfo info, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                await logger.LogModalCommandAsync(info, ctx, result);
            }
        }

        private async Task HandleInteraction(SocketInteraction socket)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of the InteractionModuleBase<T> modules
                SocketInteractionContext ctx = new(client, socket);

                await commands.ExecuteCommandAsync(ctx, services);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (socket.Type == InteractionType.ApplicationCommand)
                {
                    // Let the user know / Log into error channel
                    await socket
                        .GetOriginalResponseAsync()
                        .ContinueWith(async msg => await msg.Result.DeleteAsync());
                }
            }
        }

        private async Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                await logger.LogSlashCommandAsync(info, ctx, result);
            }
        }

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
