using Discord.Interactions;
using Discord.WebSocket;
using Wp.Api;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.ComponentInteraction;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands.Manager
{
    public class Clan : InteractionModuleBase<SocketInteractionContext>
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

        public Clan(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.CLAN_REMOVE_SELECT_MENU, runMode: RunMode.Async)]
        public async Task RemoveSelect(string[] selections)
        {
            if (!TryDecodeSelectInteraction(selections, IdProvider.CLAN_REMOVE_SELECT_MENU, out SelectSerializer selectSerializer, out SelectOptionSerializer[] optionsSerializer)) return;

            await DeferAsync();

            SelectOptionSerializer option = optionsSerializer.First();

            // Loads databases infos
            DbClans clans = Database.Context.Clans;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets clan
            ClashOfClans.Models.Clan? cClan = await ClashOfClansApi.Clans.GetByTagAsync(option.Value);

            if (cClan is null)
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

                return;
            }

            // Removes clan
            clans.Remove(c => c.Guild == dbGuild && c.Tag == cClan.Tag);

            await ModifyOriginalResponseAsync(msg => msg.Content = interactionText.ClanSelectRemoved(cClan.Name));
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private bool TryDecodeSelectInteraction(string[] selections, string selectId, out SelectSerializer selectSerializer, out SelectOptionSerializer[] optionsSerializer)
        {
            // Get SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Decodes all options
            List<SelectOptionSerializer?> options = new();

            selections
                .AsParallel()
                .ForAll(s => options.Add(SelectOptionSerializer.Decode(s)));

            selectSerializer = new(Context.User.Id, msg.Id, selectId);
            optionsSerializer = options
                .AsParallel()
                .Where(o => o != null)
                .ToArray()!;

            // Gets guild and interaction text
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            IManager interactionText = dbGuild.ManagerText;

            // Checks if there's options in the interaction
            if (optionsSerializer.Length < 1)
            {
                Task response = RespondAsync(interactionText.SelectDontContainsOption, ephemeral: true);
                response.Wait();

                return false;
            }

            // Checks if user is elligible for interaction
            if (Context.User.Id != optionsSerializer.First().UserId)
            {
                Task response = RespondAsync(interactionText.UserNotAllowedToInteract, ephemeral: true);
                response.Wait();

                return false;
            }

            ComponentStorage componentStorage = ComponentStorage.GetInstance();

            // Encodes key
            string key = selectSerializer.Encode();

            componentStorage.Selects.Remove(key);

            return true;
        }
    }
}
