using Discord;
using Discord.Interactions;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database.Services;
using Wp.Discord;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Admin
{
    [RequireUserRole(RoleType.ADMINISTRATOR)]
    [Group("role", "Role commands handler")]
    public class Role : InteractionModuleBase<SocketInteractionContext>
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

        public Role(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          MANAGER HANDLER                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [Group("manager", "Role manager commands handler")]
        public class RoleManager : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a role as a manager within the guild")]
            public async Task Add([Summary("role", "The role who will be able to do manager commands")] IRole role)
            {
                await DeferAsync(true);

                // Loads databases infos
                DbRoles roles = Database.Context.Roles;
                Guild dbGuild = Database.Context
                    .Guilds
                    .First(g => g.Id == Context.Guild.Id);

                // Filters for guild
                Common.Models.Role[] dbRoles = roles
                    .AsParallel()
                    .Where(r => r.Guild == dbGuild && r.Type == RoleType.MANAGER)
                    .ToArray();

                // Gets command responses
                IAdmin commandText = dbGuild.AdminText;

                // Checks if there's too many roles in database...
                if (dbRoles.Length >= Settings.MAX_OPTION_PER_SELECT_MENU)
                {
                    await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RoleManagerAddTooManyRoles(dbRoles.Length));

                    return;
                }

                // Checks if role is already in database
                if (dbRoles.Any(r => r.Id == role.Id))
                {
                    await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RoleManagerAddRoleAlreadyExists);

                    return;
                }

                // Adds and save
                Common.Models.Role newRole = new(dbGuild, role.Id, RoleType.MANAGER);
                roles.Add(newRole);

                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RoleManagerAddRoleAdded);
            }

            [SlashCommand("delete", "Delete a registered manager role from the guild")]
            public async Task Delete()
            {
                await DeferAsync(true);

                // Loads databases infos
                DbRoles roles = Database.Context.Roles;
                Guild dbGuild = Database.Context
                    .Guilds
                    .First(g => g.Id == Context.Guild.Id);

                // Filters for guild
                Common.Models.Role[] dbRoles = roles
                    .AsParallel()
                    .Where(r => r.Guild == dbGuild && r.Type == RoleType.MANAGER)
                    .ToArray();

                // Gets command responses
                IAdmin commandText = dbGuild.AdminText;
                IGeneralResponse generalResponses = dbGuild.GeneralResponses;

                if (!dbRoles.Any())
                {
                    await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RoleManagerDeleteNoRoles);

                    return;
                }

                // Build select menu
                SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                    .WithCustomId(IdProvider.ROLE_MANAGER_DELETE_SELECT);

                dbRoles
                    .AsParallel()
                    .Select(r => Context.Guild.Roles.FirstOrDefault(gRole => gRole.Id == r.Id))
                    .Where(r => r != null)
                    .ForAll(r =>
                    {
                        IEmote emote = r!.Emoji.Name != null ? r!.Emoji : CustomEmojis.Profile;
                        menuBuilder.AddOption(r!.Name, r.Id.ToString(), emote: emote);
                    });

                // Sort options by name
                menuBuilder.Options = menuBuilder.Options
                    .OrderBy(o => o.Label)
                    .ToList();

                // Cancel button
                ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                    .WithLabel(generalResponses.CancelButton)
                    .WithStyle(ButtonStyle.Danger)
                    .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

                // Build component
                ComponentBuilder componentBuilder = new ComponentBuilder()
                    .WithSelectMenu(menuBuilder)
                    .WithButton(cancelButtonBuilder);

                await ModifyOriginalResponseAsync(msg =>
                {
                    msg.Content = commandText.RoleManagerDeleteSelectRole;
                    msg.Components = new(componentBuilder.Build());
                });
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PLAYER HANDLER                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [Group("player", "Role player commands handler")]
        public class PlayerManager : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a role as a player within the guild")]
            public async Task Add([Summary("role", "The role who will be able to do player commands and be used as MR")] IRole role)
            {
                await DeferAsync(true);

                // Loads databases infos
                DbRoles roles = Database.Context.Roles;
                Guild dbGuild = Database.Context
                    .Guilds
                    .First(g => g.Id == Context.Guild.Id);

                // Filters for guild
                Common.Models.Role[] dbRoles = roles
                    .AsParallel()
                    .Where(r => r.Guild == dbGuild && r.Type == RoleType.PLAYER)
                    .ToArray();

                // Gets command responses
                IAdmin commandText = dbGuild.AdminText;

                // Checks if there's too many roles in database...
                if (dbRoles.Length >= Settings.MAX_OPTION_PER_SELECT_MENU)
                {
                    await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RolePlayerAddTooManyRoles(dbRoles.Length));

                    return;
                }

                // Checks if role is already in database
                if (dbRoles.Any(r => r.Id == role.Id))
                {
                    await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RolePlayerAddRoleAlreadyExists);

                    return;
                }

                // Adds and save
                Common.Models.Role newRole = new(dbGuild, role.Id, RoleType.PLAYER);
                roles.Add(newRole);

                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RolePlayerAddRoleAdded);
            }

            [SlashCommand("delete", "Delete a registered player role from the guild")]
            public async Task Delete()
            {
                await DeferAsync(true);

                // Loads databases infos
                DbRoles roles = Database.Context.Roles;
                Guild dbGuild = Database.Context
                    .Guilds
                    .First(g => g.Id == Context.Guild.Id);

                // Filters for guild
                Common.Models.Role[] dbRoles = roles
                    .AsParallel()
                    .Where(r => r.Guild == dbGuild && r.Type == RoleType.PLAYER)
                    .ToArray();

                // Gets command responses
                IAdmin commandText = dbGuild.AdminText;
                IGeneralResponse generalResponses = dbGuild.GeneralResponses;

                if (!dbRoles.Any())
                {
                    await ModifyOriginalResponseAsync(msg => msg.Content = commandText.RolePlayerDeleteNoRoles);

                    return;
                }

                // Build select menu
                SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                    .WithCustomId(IdProvider.ROLE_PLAYER_DELETE_SELECT);

                dbRoles
                    .AsParallel()
                    .Select(r => Context.Guild.Roles.FirstOrDefault(gRole => gRole.Id == r.Id))
                    .Where(r => r != null)
                    .ForAll(r =>
                    {
                        IEmote emote = r!.Emoji.Name != null ? r!.Emoji : CustomEmojis.Profile;
                        menuBuilder.AddOption(r!.Name, r.Id.ToString(), emote: emote);
                    });

                // Sort options by name
                menuBuilder.Options = menuBuilder.Options
                    .OrderBy(o => o.Label)
                    .ToList();

                // Cancel button
                ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                    .WithLabel(generalResponses.CancelButton)
                    .WithStyle(ButtonStyle.Danger)
                    .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

                // Build component
                ComponentBuilder componentBuilder = new ComponentBuilder()
                    .WithSelectMenu(menuBuilder)
                    .WithButton(cancelButtonBuilder);

                await ModifyOriginalResponseAsync(msg =>
                {
                    msg.Content = commandText.RolePlayerDeleteSelectRole;
                    msg.Components = new(componentBuilder.Build());
                });
            }
        }
    }
}
