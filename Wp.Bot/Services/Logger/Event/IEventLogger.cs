using Discord.WebSocket;

namespace Wp.Bot.Services.Logger.Event
{
    public interface IEventLogger
    {
        public Task ClientReadyAsync();

        public Task GuildJoinedAsync(SocketGuild guild);

        public Task GuildLeftAsync(SocketGuild guild);

        public Task GuildLeftAsync(ulong id);
    }
}
