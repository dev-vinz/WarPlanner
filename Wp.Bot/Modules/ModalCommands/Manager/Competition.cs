using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ModalCommands.Manager
{
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

        [ModalInteraction(CompetitionEditNameModal.ID, runMode: RunMode.Async)]
        public async Task EditName(CompetitionEditNameModal modal)
        {
            await DeferAsync(true);

            // Get SocketMessageComponent and original message
            SocketModal socket = (Context.Interaction as SocketModal)!;
            RestInteractionMessage originalMessage = await socket.GetOriginalResponseAsync();

            // Disable all components
            await originalMessage.DisableAllComponentsAsync();

            // Gets guild and interaction text
            DbCompetitions competitions = Database.Context.Competitions;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas and remove then instantly
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(originalMessage.Id, out string[]? datas) && datas?.Length != 1)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.FailToGetStorageComponentData);

                return;
            }

            // Recovers data
            ulong competitionId = ulong.Parse(datas[0]);

            Common.Models.Competition dbCompetition = competitions
                .First(c => c.Id == competitionId && c.Guild == dbGuild);

            string oldName = dbCompetition.Name;

            // Updates competition's name
            dbCompetition.Name = modal.Name;
            competitions.Update(dbCompetition);

            // Gets environment
            SocketCategoryChannel category = Context.Guild.GetCategoryChannel(dbCompetition.Id);

            // Updates environment
            string categName = category.Name.Replace(oldName, modal.Name);

            await category.ModifyAsync(categ => categ.Name = categName);
            Context.Guild.Roles
                .Where(r => r.Name.Contains(oldName))
                .ToList()
                .ForEach(async r =>
                {
                    string roleName = r.Name.Replace(oldName, modal.Name);

                    await r.ModifyAsync(gR => gR.Name = roleName);
                });

            await FollowupAsync(interactionText.EditCompetitionNameUpdated(modal.Name), ephemeral: true);
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
