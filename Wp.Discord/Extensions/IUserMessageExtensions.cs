using Discord;
using Wp.Common.Services;
using Wp.Discord.ComponentInteraction;

namespace Wp.Discord.Extensions
{
    public static class IUserMessageExtensions
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           SELECT ACTIONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Disables a select component after an interaction has been made
        /// </summary>
        /// <param name="message">The interaction response</param>
        /// <param name="select_id">The select's custom id to disable</param>
        /// <param name="limit">The time limit until the select is automatically disabled</param>
        public static async Task DisableSelectAfterSelectionAsync(this IUserMessage message, string select_id, ulong userId, TimeSpan? limit = null)
        {
            // Encodes key
            string key = new SelectSerializer(userId, message.Channel.Id, select_id).Encode();

            // Gets the component interaction storage
            ComponentStorage storage = ComponentStorage.GetInstance();

            // Removes after limit
            if (limit != null)
            {
                _ = JSFunctions.SetTimeoutAsync(() =>
                {
                    storage.Selects.Remove(key);
                }, (TimeSpan)limit);
            }

            // Adds interaction
            storage.Selects.Add(key);

            List<ActionRowBuilder> actionRows = new();

            // Gets all action rows with components inside
            message.Components
                .AsParallel()
                .Select(component => component as ActionRowComponent)
                .ForAll(row =>
                {
                    ActionRowBuilder rowBuilder = new();

                    row?.Components
                        .AsParallel()
                        .ForAll(component =>
                        {
                            if (component?.Type == ComponentType.SelectMenu)
                            {
                                SelectMenuComponent menuComponent = (component as SelectMenuComponent)!;

                                SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                    .WithCustomId(component.CustomId)
                                    .WithPlaceholder(menuComponent.Placeholder)
                                    .WithDisabled(menuComponent.IsDisabled)
                                    .WithMaxValues(menuComponent.MaxValues)
                                    .WithMinValues(menuComponent.MinValues);

                                if (menuBuilder.CustomId == select_id) menuBuilder.IsDisabled = true;

                                menuComponent
                                    .Options
                                    .AsParallel()
                                    .ForAll(o =>
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
                .WithRows(actionRows);

            // Wait till it's interacted or limit reached
            while (storage.Selects.Contains(key)) { };

            try
            {
                await message.ModifyAsync(msg =>
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
