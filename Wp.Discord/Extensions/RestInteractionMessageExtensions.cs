using Discord;
using Discord.Rest;

namespace Wp.Discord.Extensions
{
    public static class RestInteractionMessageExtensions
    {
        /// <summary>
        /// Disables a button component after an interaction has been made
        /// </summary>
        /// <param name="msg">The original message interaction</param>
        public static async Task DisableAllComponentsAsync(this RestInteractionMessage msg)
        {
            List<ActionRowBuilder> actionRows = new();

            // Gets all action rows with components inside
            msg.Components
                .ToList()
                .ForEach(row =>
                {
                    ActionRowBuilder rowBuilder = new();

                    row?.Components
                        .ToList()
                        .ForEach(component =>
                        {
                            if (component?.Type == ComponentType.Button)
                            {
                                ButtonComponent buttonComponent = (component as ButtonComponent)!;

                                ButtonBuilder buttonBuilder = new ButtonBuilder()
                                    .WithCustomId(buttonComponent.CustomId)
                                    .WithDisabled(buttonComponent.IsDisabled)
                                    .WithEmote(buttonComponent.Emote)
                                    .WithLabel(buttonComponent.Label)
                                    .WithStyle(buttonComponent.Style)
                                    .WithUrl(buttonComponent.Url);

                                buttonBuilder.IsDisabled = true;

                                rowBuilder.WithButton(buttonBuilder);
                            }
                            else if (component?.Type == ComponentType.SelectMenu)
                            {
                                SelectMenuComponent menuComponent = (component as SelectMenuComponent)!;

                                SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                    .WithCustomId(component.CustomId)
                                    .WithPlaceholder(menuComponent.Placeholder)
                                    .WithDisabled(menuComponent.IsDisabled)
                                    .WithMaxValues(menuComponent.MaxValues)
                                    .WithMinValues(menuComponent.MinValues);

                                menuBuilder.IsDisabled = true;

                                menuComponent
                                    .Options
                                    .ToList()
                                    .ForEach(o =>
                                    {
                                        menuBuilder.AddOption(o.Label, o.Value, o.Description, o.Emote, o.IsDefault);
                                    });

                                rowBuilder.WithSelectMenu(menuBuilder);
                            }
                            else
                            {
                                rowBuilder.AddComponent(component);
                            }
                        });

                    actionRows.Add(rowBuilder);
                });

            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithRows(actionRows.Where(row => row.Components.Any()));

            try
            {
                await msg.ModifyAsync(msg =>
                {
                    msg.Components = new(componentBuilder.Build());
                });
            }
            catch (Exception)
            {
                // Try to modify a deleted message - Do nothing
            }
        }
    }
}
