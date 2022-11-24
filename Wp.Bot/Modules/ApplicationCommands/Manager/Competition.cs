using Discord;
using Discord.Interactions;
using System.Text;
using Wp.Api;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Manager
{
    [Group("competition", "Competition commands handler")]
    public class Competition : InteractionModuleBase<SocketInteractionContext>
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

        public Competition(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [SlashCommand("add", "Register a new competition within the guild", runMode: RunMode.Async)]
        public async Task Add([Summary("name", "A competition's name")] string name)
        {
            await DeferAsync();

            if (Encoding.UTF8.GetByteCount(name) != name.Length)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = "Pas d'emojis");

                return;
            }

            // Loads databases infos
            DbClans clans = Database.Context.Clans;
            DbCompetitions competitions = Database.Context.Competitions;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Filters for guild
            Common.Models.Clan[] dbClans = clans.AsParallel().Where(c => c.Guild == dbGuild).ToArray();

            // Gets command responses
            IManager commandText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Checks if there's at least one clan registered
            if (!dbClans.Any())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = commandText.NoClanRegisteredToRemove);

                return;
            }

            // Checks Clash Of Clans API
            if (!await ClashOfClansApi.TryAccessApiAsync())
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

                return;
            }

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.COMPETITION_ADD_SELECT_MAIN_CLAN);

            dbClans
                .AsParallel()
                .ForAll(c =>
                {
                    SelectOptionSerializer optionSerializer = new(dbGuild.Id, Context.User.Id, c.Tag, name);
                    menuBuilder.AddOption(c.Profile.Name, optionSerializer.ToString(), c.Tag);
                });

            // Sort options by name
            menuBuilder.Options = menuBuilder.Options
                .AsParallel()
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

            IUserMessage message = await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = commandText.ChooseCompetitionMainClan;
                msg.Components = new(componentBuilder.Build());
            });

            message.DisableSelectAfterSelection(menuBuilder.CustomId, Context.User.Id, removeButtons: true);
            message.DeleteAllComponentsAfterButtonClick(cancelButtonBuilder.CustomId, Context.User.Id);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



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
