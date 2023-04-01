using Discord;
using Discord.WebSocket;
using Wp.Common.Models;
using Wp.Common.Services.NodaTime;
using Wp.Database.Settings;
using Color = Discord.Color;
using TZone = Wp.Common.Models.TimeZone;

namespace Wp.Bot.Services.Logger.Event
{
	public class EventLogger : IEventLogger
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

		public EventLogger(DiscordSocketClient client)
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

		public async Task ClientReadyAsync()
		{
			string title = "Bot connected";
			Color color = new(51, 83, 121);
			string description = "I've just logged on !";
			string guildThumbnail = client.CurrentUser.GetAvatarUrl();

			EventLoggerMessage message = new(title, color, description, null, guildThumbnail);

			await LogEventToChannelAsync(message);
		}

		public async Task EventExecutionFailedAsync(Exception e)
		{
			string title = "Time loop error";
			Color color = new(173, 52, 62);
			string description = e.Message;
			string guildThumbnail = client.CurrentUser.GetAvatarUrl();

			EventLoggerMessage message = new(title, color, description, null, guildThumbnail);

			await LogEventToChannelAsync(message);
		}

		public async Task EventExecutionStartedAsync()
		{
			string title = "Time loop";
			Color color = new(67, 197, 158);
			string description = "The time loop has just started";
			string guildThumbnail = client.CurrentUser.GetAvatarUrl();

			EventLoggerMessage message = new(title, color, description, null, guildThumbnail);

			await LogEventToChannelAsync(message);
		}

		public async Task GuildJoinedAsync(SocketGuild guild)
		{
			string title = "Guild joined";
			Color color = new(143, 201, 163);
			string description = "I've joined a new guild";
			string guildName = guild.Name;
			string guildThumbnail = guild.IconUrl;

			EventLoggerMessage message = new(title, color, description, guildName, guildThumbnail);

			await LogEventToChannelAsync(message);
		}

		public async Task GuildLeftAsync(SocketGuild guild)
		{
			string title = "Guild left";
			Color color = new(255, 128, 64);
			string description = "I've left a guild";
			string guildName = guild.Name;
			string guildThumbnail = guild.IconUrl;

			EventLoggerMessage message = new(title, color, description, guildName, guildThumbnail);

			await LogEventToChannelAsync(message);
		}

		public async Task GuildLeftAsync(ulong id)
		{
			string title = "Guild left";
			Color color = new(255, 128, 64);
			string description = "I've left a guild";
			string guildName = id.ToString();
			string guildThumbnail = client.CurrentUser.GetAvatarUrl();

			EventLoggerMessage message = new(title, color, description, guildName, guildThumbnail);

			await LogEventToChannelAsync(message);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private async Task LogEventToChannelAsync(EventLoggerMessage message)
		{
			IChannel channel = await client.GetChannelAsync(Configurations.DEV_LOG_CHANNEL_ID);

			if (channel is not ITextChannel logChannel)
			{
				LogMessage logMessage = new(LogSeverity.Warning, "Discord", "Log channel didn't found");
				Console.WriteLine(logMessage.ToString());

				return;
			}

			NodaConverter converter = new();

			DateTimeOffset now = converter.ConvertNowTo(TZone.CET_CEST);

			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithTitle(message.Title)
				.WithThumbnailUrl(message.GuildThumbnail)
				.WithColor(message.Color)
				.WithDescription(message.Description)
				.WithFooter($"{now.Year} © {client.CurrentUser.Username}");

			if (message.Guild != null)
			{
				embedBuilder.AddField("Guild", message.Guild, true);
			}

			embedBuilder.AddField("Date", $"{now:dddd dd MMMM yyyy}")
				.AddField("Time", $"{now:HH:mm:ss} {TZone.CET_CEST.AsAttribute().DisplayName}");

			await logChannel.SendMessageAsync(embed: embedBuilder.Build());
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
