using Discord;
using Discord.WebSocket;
using Wp.Discord.ComponentInteraction;

namespace Wp.Discord.Extensions
{
    public static class SocketInteractionExtensions
    {
        public static async Task DisableComponentsAsync(this SocketInteraction interaction, bool allSameType = false, bool allComponents = false, bool ephemeral = true)
        {
            await interaction.DeferAsync(ephemeral);

            // Get SocketMessageComponent and original message
            SocketMessageComponent socket = (interaction as SocketMessageComponent)!;
            SocketUserMessage originalMessage = socket.Message;

            // Gets informations about component interacted
            ComponentType componentType = socket.Data.Type;
            string customId = socket.Data.CustomId;

            // Gets the component interaction storage
            ComponentStorage storage = ComponentStorage.GetInstance();

            List<ActionRowBuilder> actionRows = new();

            // Gets all action rows with components inside
            originalMessage.Components
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

                                if (allComponents) buttonBuilder.IsDisabled = true;
                                if (allSameType && componentType == ComponentType.Button) buttonBuilder.IsDisabled = true;
                                if (componentType == ComponentType.Button && customId == component.CustomId) buttonBuilder.IsDisabled = true;

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

                                if (allComponents) menuBuilder.IsDisabled = true;
                                if (allSameType && componentType == ComponentType.SelectMenu) menuBuilder.IsDisabled = true;
                                if (componentType == ComponentType.SelectMenu && customId == component.CustomId) menuBuilder.IsDisabled = true;

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
                await interaction.ModifyOriginalResponseAsync(msg =>
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
