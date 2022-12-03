using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Wp.Common.Models;
using Wp.Common.Services.NodaTime;
using Wp.Database.Settings;
using TZone = Wp.Common.Models.TimeZone;

namespace Wp.Bot.Services.Logger
{
    public class CommandLogger : ICommandLogger
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly DiscordSocketClient client;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public CommandLogger(DiscordSocketClient client)
        {
            // Inputs
            {
                this.client = client;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private async Task LogErrorToChannelAsync(LogMessage message)
        {
            IChannel channel = await client.GetChannelAsync(Configurations.DEV_LOG_CHANNEL_ID);

            if (channel is not ITextChannel logChannel)
            {
                Console.Error.WriteLine("[WARNING] Log channel didn't found");

                return;
            }

            NodaConverter converter = new();
            DateTimeOffset now = converter.ConvertNowTo(TZone.CET_CEST);

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(message.Title)
                .WithThumbnailUrl(message.GuildThumbnail)
                .WithColor(message.Color)
                .WithDescription("An error has occured...")
                .AddField("Guild", message.Guild, true)
                .AddField("Command", message.Command, true)
                .AddField("Date", $"{now:dddd dd MMMM yyyy}")
                .AddField("Time", $"{now:HH:mm:ss} {TZone.CET_CEST.AsAttribute().DisplayName}")
                .AddField("Exception Type", message.Exception)
                .AddField("Message", message.Error)
                .WithFooter($"{now.Year} © {client.CurrentUser.Username}");

            await logChannel.SendMessageAsync(embed: embedBuilder.Build());
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public async Task LogComponentCommandAsync(ComponentCommandInfo info, IInteractionContext ctx, IResult result)
        {
            Color embedColor = result.Error switch
            {
                InteractionCommandError.UnknownCommand => new Color(66, 0, 84),
                InteractionCommandError.ConvertFailed => new Color(89, 0, 113),
                InteractionCommandError.BadArgs => new Color(112, 0, 142),
                InteractionCommandError.Exception => new Color(135, 0, 170),
                InteractionCommandError.Unsuccessful => new Color(158, 46, 187),
                InteractionCommandError.UnmetPrecondition => new Color(181, 92, 204),
                InteractionCommandError.ParseFailed => new Color(204, 138, 221),
                _ => new Color(227, 184, 238),
            };

            string guild = ctx.Guild.Name;
            string thumbnail = ctx.Guild.IconUrl;
            string command = info.ToString();
            string exception = result.Error.ToString() ?? "Default";

            LogMessage message = new("Component Command Error", embedColor, guild, thumbnail, command, exception, result.ErrorReason);

            await LogErrorToChannelAsync(message);
        }

        public async Task LogModalCommandAsync(ModalCommandInfo info, IInteractionContext ctx, IResult result)
        {
            Color embedColor = result.Error switch
            {
                InteractionCommandError.UnknownCommand => new Color(20, 97, 62),
                InteractionCommandError.ConvertFailed => new Color(28, 135, 86),
                InteractionCommandError.BadArgs => new Color(36, 173, 110),
                InteractionCommandError.Exception => new Color(42, 213, 135),
                InteractionCommandError.Unsuccessful => new Color(80, 221, 157),
                InteractionCommandError.UnmetPrecondition => new Color(118, 229, 179),
                InteractionCommandError.ParseFailed => new Color(156, 237, 201),
                _ => new Color(194, 245, 223),
            };

            string guild = ctx.Guild.Name;
            string thumbnail = ctx.Guild.IconUrl;
            string command = info.ToString();
            string exception = result.Error.ToString() ?? "Default";

            LogMessage message = new("Modal Command Error", embedColor, guild, thumbnail, command, exception, result.ErrorReason);

            await LogErrorToChannelAsync(message);
        }

        public async Task LogSlashCommandAsync(SlashCommandInfo info, IInteractionContext ctx, IResult result)
        {
            Color embedColor = result.Error switch
            {
                InteractionCommandError.UnknownCommand => new Color(102, 11, 0),
                InteractionCommandError.ConvertFailed => new Color(139, 15, 0),
                InteractionCommandError.BadArgs => new Color(178, 19, 0),
                InteractionCommandError.Exception => new Color(214, 22, 0),
                InteractionCommandError.Unsuccessful => new Color(222, 64, 46),
                InteractionCommandError.UnmetPrecondition => new Color(230, 106, 92),
                InteractionCommandError.ParseFailed => new Color(238, 148, 138),
                _ => new Color(246, 190, 184),
            };

            string guild = ctx.Guild.Name;
            string thumbnail = ctx.Guild.IconUrl;
            string command = info.ToString();
            string exception = result.Error.ToString() ?? "Default";

            LogMessage message = new("Slash Command Error", embedColor, guild, thumbnail, command, exception, result.ErrorReason);

            await LogErrorToChannelAsync(message);
        }

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
