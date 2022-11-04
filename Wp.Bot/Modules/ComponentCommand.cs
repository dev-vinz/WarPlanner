using Discord.Interactions;

namespace Wp.Bot.Modules
{
    public class ComponentCommand : InteractionModuleBase<SocketInteractionContext>
    {
        // This is the handler for the button created above. It is triggered by nmatching the customID of the button.
        [ComponentInteraction("button1")]
        public async Task ButtonHandler()
        {
            // try setting a breakpoint here to see what kind of data is supplied in a ComponentInteraction.
            var c = Context;
            await RespondAsync($"You pressed a button!");
        }

        // SelectMenu interaction handler. This receives an array of the selections made.
        [ComponentInteraction("menu1")]
        public async Task MenuHandler(string[] selections)
        {
            var m = await Context.Interaction.GetOriginalResponseAsync();

            // For the sake of demonstration, we only want the first value selected.
            await RespondAsync($"You selected {selections.First()}", ephemeral: true);
        }
    }
}
