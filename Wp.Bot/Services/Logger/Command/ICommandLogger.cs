using Discord;
using Discord.Interactions;

namespace Wp.Bot.Services.Logger.Command
{
    public interface ICommandLogger
    {
        public Task LogComponentCommandAsync(ComponentCommandInfo info, IInteractionContext ctx, IResult result);

        public Task LogModalCommandAsync(ModalCommandInfo info, IInteractionContext ctx, IResult result);

        public Task LogSlashCommandAsync(SlashCommandInfo info, IInteractionContext ctx, IResult result);
    }
}
