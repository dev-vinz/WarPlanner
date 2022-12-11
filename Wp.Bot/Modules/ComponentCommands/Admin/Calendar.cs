using Discord.Interactions;
using Wp.Bot.Services;
using Wp.Discord.Extensions;

namespace Wp.Bot.Modules.ComponentCommands.Admin
{
    public class Calendar : InteractionModuleBase<SocketInteractionContext>
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

        public Calendar(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          BUTTON COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_CHOOSE_REMIND_WAR, runMode: RunMode.Async)]
        public async Task ChooseRemind()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Choose remind", ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY, runMode: RunMode.Async)]
        public async Task Display()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Enable / Disable display", ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_DISPLAY_FREQUENCY, runMode: RunMode.Async)]
        public async Task DisplayFrequency()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Display frequency", ephemeral: true);
        }

        [ComponentInteraction(IdProvider.CALENDAR_OPTIONS_BUTTON_REMIND_WAR, runMode: RunMode.Async)]
        public async Task RemindWar()
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("Enable / Disable remind war", ephemeral: true);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    }
}
