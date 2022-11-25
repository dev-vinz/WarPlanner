using Discord;
using System.Collections.Concurrent;
using Wp.Common.Services;
using Wp.Discord.ComponentInteraction;

namespace Wp.Discord.Extensions
{
    public static class IUserMessageExtensions
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly ConcurrentDictionary<string, string> ABORT_EVENT = new();
        private static readonly string DEFAULT_ABORT_VALUE = string.Empty;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           BUTTON ACTIONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Delete all components after an interaction with a button component
        /// </summary>
        /// <param name="message">The interaction response</param>
        /// <param name="buttonId">The button's custom id to trigger</param>
        public static void DeleteAllComponentsAfterButtonClick(this IUserMessage message, string buttonId, ulong userId)
        {
            new Thread(async () =>
            {
                // Encodes key
                string key = new ButtonSerializer(userId, message.Id, buttonId).Encode();

                // Gets the component interaction storage
                ComponentStorage storage = ComponentStorage.GetInstance();

                // Adds interaction
                storage.Buttons.TryAdd(key, userId);

                // Wait till it's interacted or limit reached
                while (storage.Buttons.ContainsKey(key)) { };

                // In doubt, remove all component datas associated at this message
                storage.MessageDatas.TryRemove(message.Id, out string[] _);

                if (!ABORT_EVENT.ContainsKey(key))
                {
                    // Remove all other interactions...
                    // ...buttons
                    storage
                        .Buttons
                        .Keys
                        .AsParallel()
                        .Where(key => key.Contains(message.Id.ToString()))
                        .ForAll(key => storage.Buttons.TryRemove(key, out ulong _));

                    // ...selects
                    storage
                        .Selects
                        .AsParallel()
                        .Where(key => key.Contains(message.Id.ToString()))
                        .ForAll(key => storage.Selects.Remove(key));

                    try
                    {
                        await message.ModifyAsync(msg =>
                        {
                            msg.Components = new(new ComponentBuilder().Build());
                        });
                    }
                    catch (Exception)
                    {
                        // Try to modify a deleted message - Do nothing
                        // /!\ Message has to be not ephemeral /!\
                    }
                }

                ABORT_EVENT.TryRemove(key, out string _);
            }).Start();
        }

        /// <summary>
        /// Disables a button component after an interaction has been made
        /// </summary>
        /// <param name="message">The interaction response</param>
        /// <param name="buttonId">The button's custom id to disable</param>
        /// <param name="limit">The time limit until the button is automatically disabled</param>
        /// <param name="disableAll">A flag that indicates wether all other buttons have to be disabled</param>
        /// <param name="disableAllComponents">A flag that indicates wether all other components have to be disabled</param>
        public static void DisableButtonAfterClick(this IUserMessage message, string buttonId, ulong userId, TimeSpan? limit = null, bool disableAll = false, bool disableAllComponents = false)
        {
            new Thread(async () =>
            {
                // Encodes key
                string key = new ButtonSerializer(userId, message.Id, buttonId).Encode();

                // Gets the component interaction storage
                ComponentStorage storage = ComponentStorage.GetInstance();

                if (limit != null)
                {
                    _ = JSFunctions.SetTimeoutAsync(() =>
                    {
                        storage.Buttons.TryRemove(key, out ulong _);
                    }, (TimeSpan)limit);
                }

                // Adds interaction
                storage.Buttons.TryAdd(key, userId);

                List<ActionRowBuilder> actionRows = new();

                // Gets all action rows with components inside
                message.Components
                    .Select(component => component as ActionRowComponent)
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

                                    if (disableAll || disableAllComponents) buttonBuilder.IsDisabled = true;
                                    if (buttonBuilder.CustomId == buttonId) buttonBuilder.IsDisabled = true;

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

                                    if (disableAllComponents) menuBuilder.IsDisabled = true;

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

                // Wait till it's interacted or limit reached
                while (storage.Buttons.ContainsKey(key)) { };

                if (!ABORT_EVENT.ContainsKey(key))
                {
                    // If disable all, remove all other button interactions
                    if (disableAll)
                    {
                        storage
                            .Buttons
                            .Keys
                            .AsParallel()
                            .Where(key => key.Contains(message.Id.ToString()))
                            .ForAll(key =>
                            {
                                ABORT_EVENT.TryAdd(key, DEFAULT_ABORT_VALUE);
                                storage.Buttons.TryRemove(key, out ulong _);
                            });
                    }

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
                        // /!\ Message has to be not ephemeral /!\
                    }
                }

                ABORT_EVENT.TryRemove(key, out string _);
            }).Start();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           SELECT ACTIONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Disables a select component after an interaction has been made
        /// </summary>
        /// <param name="message">The interaction response</param>
        /// <param name="selectId">The select's custom id to disable</param>
        /// <param name="limit">The time limit until the select is automatically disabled</param>
        /// <param name="disableAll">A flag that indicates wether all other selects have to be disabled</param>
        /// <param name="removeButtons">A flag that indicates wether buttons have to be removed</param>
        public static void DisableSelectAfterSelection(this IUserMessage message, string selectId, ulong userId, TimeSpan? limit = null, bool disableAll = false, bool removeButtons = false)
        {
            new Thread(async () =>
            {
                // Encodes key
                string key = new SelectSerializer(userId, message.Id, selectId).Encode();

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
                    .Select(component => component as ActionRowComponent)
                    .ToList()
                    .ForEach(row =>
                    {
                        ActionRowBuilder rowBuilder = new();

                        row?.Components
                            .ToList()
                            .ForEach(component =>
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

                                    if (disableAll) menuBuilder.IsDisabled = true;
                                    if (menuBuilder.CustomId == selectId) menuBuilder.IsDisabled = true;

                                    menuComponent
                                        .Options
                                        .ToList()
                                        .ForEach(o =>
                                        {
                                            menuBuilder.AddOption(o.Label, o.Value, o.Description, o.Emote, o.IsDefault);
                                        });

                                    rowBuilder.WithSelectMenu(menuBuilder);
                                }
                                else if (component?.Type == ComponentType.Button)
                                {
                                    if (!removeButtons)
                                    {
                                        rowBuilder.AddComponent(component);
                                    }
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

                // Wait till it's interacted or limit reached
                while (storage.Selects.Contains(key)) { };

                if (!ABORT_EVENT.ContainsKey(key))
                {
                    // If disable all, remove all other select interactions
                    if (disableAll)
                    {
                        storage
                            .Selects
                            .AsParallel()
                            .Where(key => key.Contains(message.Id.ToString()))
                            .ForAll(key =>
                            {
                                ABORT_EVENT.TryAdd(key, DEFAULT_ABORT_VALUE);
                                storage.Selects.Remove(key);
                            });
                    }

                    // If remove button, remove all button interactions
                    if (removeButtons)
                    {
                        storage
                            .Buttons
                            .Keys
                            .AsParallel()
                            .Where(key => key.Contains(message.Id.ToString()))
                            .ForAll(key =>
                            {
                                ABORT_EVENT.TryAdd(key, DEFAULT_ABORT_VALUE);
                                storage.Buttons.TryRemove(key, out ulong _);
                            });
                    }

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
                        // /!\ Message has to be not ephemeral /!\
                    }
                }

                ABORT_EVENT.TryRemove(key, out string _);
            }).Start();
        }
    }
}
