using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Wp.Api.Settings;
using Wp.Bot.Services;
using Wp.Database.Settings;

namespace Wp.Bot
{
    public class Program
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private DiscordSocketClient? client;
        private InteractionService? commands;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public async Task MainAsync()
        {
            // Call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using ServiceProvider services = ConfigureServices();

            // Get the client and assign to client 
            // You get the services via GetRequiredService<T>
            client = services.GetRequiredService<DiscordSocketClient>();
            commands = services.GetRequiredService<InteractionService>();

            // Setup logging and the ready event
            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            commands.Log += LogAsync;

            // This is where we get the Token value from the configuration file, and start the bot
            await client.LoginAsync(TokenType.Bot, Keys.DISCORD_BOT_TOKEN);
            await client.StartAsync();

            // We get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
            await services.GetRequiredService<CommandHandler>().InitializeAsync();

            // Same for EventHandler
            await services.GetRequiredService<Services.EventHandler>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            if (IsDebug)
            {
                Console.WriteLine($"DEBUG MODE : Adding commands to {Configurations.DEV_GUILD_ID}...");
                await commands!.RegisterCommandsToGuildAsync(Configurations.DEV_GUILD_ID);
            }
            else
            {
                await client!.SetGameAsync("Clash Of Clans");
                await commands!.RegisterCommandsGloballyAsync(true);
            }

            Console.WriteLine($"Connected as [{client!.CurrentUser}]");
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                //.AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true,
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandHandler>()
                .AddSingleton<Services.EventHandler>()
                .BuildServiceProvider();
        }

        public static Task Main(string[] args) => new Program().MainAsync();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    }
}