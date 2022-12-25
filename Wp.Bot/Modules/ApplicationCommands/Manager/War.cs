using Discord.Interactions;
using Discord.WebSocket;
using Wp.Api;
using Wp.Bot.Modules.ApplicationCommands.AutoCompletion;
using Wp.Bot.Services;
using Wp.Common.Models;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Discord.Extensions;
using Wp.Language;

namespace Wp.Bot.Modules.ApplicationCommands.Manager
{
	[Group("war", "War commands handler")]
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

		[SlashCommand("add", "Register a new war within the calendar", runMode: RunMode.Async)]
		public async Task Add([Summary("opponent")] string opponentTag,
							  [Summary("preparation", "How long the preparation time should be"), Autocomplete(typeof(WarPreparationAutocompleteHandler))] int warPreparation,
							  [Summary("war", "How long the war time should be"), Autocomplete(typeof(WarDurationAutocompleteHandler))] int warDuration)
		{
			await DeferAsync(true);

			// Loads databases infos
			DbCalendars calendars = Database.Context.Calendars;
			DbCompetitions competitions = Database.Context.Competitions;
			DbPlayers players = Database.Context.Players;
			Guild dbGuild = Database.Context
				.Guilds
				.First(g => g.Id == Context.Guild.Id);

			// Filters for guild
			Calendar dbCalendar = calendars
				.AsParallel()
				.First(c => c.Guild == dbGuild);

			Common.Models.Competition[] dbCompetitions = competitions
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			Player[] dbPlayers = players
				.AsParallel()
				.Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
				.ToArray();

			// Gets command responses
			IAdmin adminResponses = dbGuild.AdminText;
			IManager commandText = dbGuild.ManagerText;
			IGeneralResponse generalResponses = dbGuild.GeneralResponses;

			int totalTime = warPreparation + warDuration;

			// Gets Clash of Clans opponent clan
			ClashOfClans.Models.Clan? cOpponent = await ClashOfClansApi.Clans.GetByTagAsync(opponentTag);

			if (cOpponent is null)
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = generalResponses.ClashOfClansError);

				return;
			}

			if (dbCalendar is null)
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = adminResponses.CalendarIdNotSet);

				return;
			}

			if (!dbCompetitions.Any())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = "aucune compétition");

				return;
			}

			// Gets all guild members
			IReadOnlyCollection<SocketGuildUser> allMembers = Context.Guild.Users;

			// Filters one more time
			Player[] availablePlayers = dbPlayers
				.AsParallel()
				.Where(p =>
				{
					SocketGuildUser? user = allMembers.FirstOrDefault(m => m.Id == p.Id);

					return user is not null && user.IsAPlayer() && p.Account.TownHallLevel >= dbGuild.MinimalTownHallLevel;
				})
				.ToArray();

			if (!availablePlayers.Any())
			{
				await ModifyOriginalResponseAsync(msg => msg.Content = "pas de joueurs");

				return;
			}
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
