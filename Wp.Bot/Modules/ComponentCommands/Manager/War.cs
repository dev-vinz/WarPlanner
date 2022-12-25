using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ComponentCommands.Manager
{
    public class War : InteractionModuleBase<SocketInteractionContext>
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

        public War(CommandHandler handler)
        {
            this.handler = handler;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          SELECT COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        [ComponentInteraction(IdProvider.WAR_ADD_SELECT_HOUR)]
        public async Task AddHour(string[] selections)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            // Gets SocketMessageComponent and original message
            SocketMessageComponent socket = (Context.Interaction as SocketMessageComponent)!;
            SocketUserMessage msg = socket.Message;

            // Loads databases infos
            DbCompetitions competitions = Database.Context.Competitions;
            Guild dbGuild = Database.Context
                .Guilds
                .First(g => g.Id == Context.Guild.Id);

            // Gets interaction texts
            IManager interactionText = dbGuild.ManagerText;
            IGeneralResponse generalResponses = dbGuild.GeneralResponses;

            // Gets component datas
            ComponentStorage storage = ComponentStorage.GetInstance();
            if (!storage.MessageDatas.TryRemove(msg.Id, out string[]? datas) && datas?.Length != 3)
            {
                await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

                return;
            }

            // Recovers datas
            string opponentTag = datas[0];
            string totalTime = datas[1];

            int hours = int.Parse(selections.First());
            DateTimeOffset warDate = DateTimeOffset.Parse(datas[2]).AddHours(hours);

            CultureInfo cultureInfo = dbGuild.Language.GetCultureInfo();

            // Build select menu
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithCustomId(IdProvider.WAR_ADD_SELECT_MINUTES);

            Enumerable.Range(0, 4)
                .ToList()
                .ForEach(n => menuBuilder.AddOption(warDate.AddMinutes(n * 15).ToString("t", cultureInfo), n.ToString()));

            // Cancel button
            ButtonBuilder cancelButtonBuilder = new ButtonBuilder()
                .WithLabel(generalResponses.CancelButton)
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(IdProvider.GLOBAL_CANCEL_BUTTON);

            // Build component
            ComponentBuilder componentBuilder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder)
                .WithButton(cancelButtonBuilder);

            IUserMessage message = await FollowupAsync(interactionText.WarAddHourSelectMinutes(warDate.ToString("t", cultureInfo)), components: componentBuilder.Build(), ephemeral: true);

            // Inserts new datas
            datas = new[] { opponentTag, totalTime.ToString(), warDate.ToString() };
            storage.MessageDatas.TryAdd(message.Id, datas);
        }

        [ComponentInteraction(IdProvider.WAR_ADD_SELECT_MINUTES)]
        public async Task AddMinutes(string[] selection)
        {
            await Context.Interaction.DisableComponentsAsync(allComponents: true);

            await FollowupAsync("TODO", ephemeral: true);
        }
    }
}
