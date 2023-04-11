using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Wp.Api;
using Wp.Api.Models;
using Wp.Bot.Modules.ModalCommands.Modals;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Discord.ComponentInteraction;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ModalCommands.Manager
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
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[ModalInteraction(WarEditOpponentModal.ID, runMode: RunMode.Async)]
		public async Task EditName(WarEditOpponentModal modal)
		{
			await DeferAsync(true);

			// Gets SocketMessageComponent and original message
			SocketModal socket = (Context.Interaction as SocketModal)!;
			RestInteractionMessage originalMessage = await socket.GetOriginalResponseAsync();

			// Disable all components
			await originalMessage.DisableAllComponentsAsync();

			// Gets guild and interaction text
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			Calendar dbCalendar = Database.Context
				.Calendars
				.First(c => c.Guild == dbGuild);

			IManager interactionText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			// Gets component datas and remove then instantly
			ComponentStorage storage = ComponentStorage.GetInstance();
			if (!storage.MessageDatas.TryRemove(originalMessage.Id, out string[]? datas) && datas?.Length != 1)
			{
				await FollowupAsync(generalResponses.FailToGetStorageComponentData, ephemeral: true);

				return;
			}

			// Recovers data
			string eventId = datas[0];
			CalendarEvent warEvent = (await GoogleCalendarApi.Events.GetAsync(dbCalendar.Id, eventId))!;

			// Checks Clash Of Clans tag
			ClashOfClans.Models.Clan? cClan = await ClashOfClansApi.Clans.GetByTagAsync(modal.Tag);

			if (cClan == null)
			{
				await FollowupAsync(generalResponses.ClashOfClansError, ephemeral: true);

				return;
			}

			// Updates war's opponent
			warEvent.OpponentTag = cClan.Tag;

			if (!await GoogleCalendarApi.Events.UpdateAsync(warEvent, dbCalendar.Id))
			{
				await FollowupAsync(generalResponses.GoogleCannotUpdateEvent, ephemeral: true);

				return;
			}

			await FollowupAsync(interactionText.WarEditOpponentUpdated(cClan.Name), ephemeral: true);
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
