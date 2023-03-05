using Discord;
using Wp.Api;
using Wp.Api.Extensions;
using Wp.Api.Models;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Discord;
using Wp.Discord.Extensions;

namespace Wp.Bot.Modules.TimeEvents.War
{
	public static class Status
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static async void Execute(IGuild guild)
		{
			// Loads databases infos			
			DbCalendars calendars = Context.Calendars;
			DbCompetitions competitions = Context.Competitions;
			DbPlayers players = Context.Players;
			DbTimes times = Context.Times;
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == guild.Id);

			// Filters for guild
			Common.Models.Calendar? dbCalendar = calendars
				.AsParallel()
				.FirstOrDefault(c => c.Guild == dbGuild);

			Competition[] dbCompetitions = competitions
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			Player[] dbPlayers = players
				.AsParallel()
				.Where(p => p.Guild == dbGuild || p.Guild.Id == Configurations.DEV_GUILD_ID)
				.ToArray();

			Time? dbTime = times
				.AsParallel()
				.FirstOrDefault(t => t.Guild == dbGuild && t.Action == TimeAction.REMIND_WAR_STATUS);

			// Makes some verifications
			if (dbGuild.PremiumLevel < PremiumLevel.MEDIUM || dbCalendar == null || dbTime == null) return;

			if (!dbTime.IsScanAllowed()) return;

			// Update time
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;

			dbTime.Date = utcNow;
			times.Update(dbTime);

			// Gets all events
			CalendarEvent[] calendarEvents = await GoogleCalendarApi.Events.ListAsync(dbCalendar.Id);

			// Gets all guild users
			IReadOnlyCollection<IGuildUser> users = await guild.GetUsersAsync();

			// Filters events and warn managers and players
			calendarEvents
				.AsParallel()
				.Where(e =>
				{
					TimeSpan timeSpan = e.Start - dbGuild.Now;
					int nbMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);
					return nbMinutes == Settings.CALENDAR_REMIND_STATUS;
				})
				.Where(e => dbCompetitions.Any(c => c.Name == e.CompetitionName))
				.ForAll(@event =>
				{
					Competition competition = dbCompetitions.First(c => c.Name == @event.CompetitionName);

					Player[] competPlayers = @event.Players
						.AsParallel()
						.Select(tag => dbPlayers.FirstOrDefault(p => p.Tag == tag))
						.Where(p => p != null)
						.ToArray()!;

					// Warns managers
					users
						.AsParallel()
						.Where(u => u.IsAManager())
						.ForAll(async manager => await WarnManagerAsync(manager, competPlayers, competition));

					competPlayers
						.AsParallel()
						.ForAll(async player => await WarnPlayerAsync(guild, player, competition));
				});
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static async Task WarnManagerApiOfflineAsync(IGuildUser manager, Competition competition)
		{
			// Gets text
			string warnText = competition.Guild.TimeText.WarRemindStatusManagerApiOffline(competition.Name);

			try
			{
				await manager.SendMessageAsync(warnText);
			}
			catch (Exception)
			{
				// Manager doesn't accept DMs or bot is blocked
			}
		}

		private static async Task WarnPlayerApiOfflineAsync(IGuildUser user, Player player, Competition competition)
		{
			// Gets text
			string warnText = competition.Guild.TimeText.WarRemindStatusPlayerApiOffline(user.DisplayName, competition.Name);

			try
			{
				await user.SendMessageAsync(warnText);
			}
			catch (Exception)
			{
				// User doesn't accept DMs or bot is blocked
			}
		}

		private static async Task WarnManagerAsync(IGuildUser manager, Player[] players, Competition dbCompetition)
		{
			// Checks that Clash of Clans API is online
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await WarnManagerApiOfflineAsync(manager, dbCompetition);

				return;
			}

			// Gets players
			string[] competPlayers = players
				.AsParallel()
				.Select(p => p.Account)
				.OrderByDescending(p => p.TownHallLevel)
				.ThenBy(p => p.Name)
				.Select(p => $"{CustomEmojis.ParseTownHallLevel(p.TownHallLevel)} **{p.Name}** `{p.Tag}`")
				.ToArray();

			// Gets text
			string warnText = dbCompetition.Guild.TimeText.WarRemindStatusWarnManager(string.Join("\n", competPlayers), dbCompetition.Name);

			try
			{
				await manager.SendMessageAsync(warnText);
			}
			catch (Exception)
			{
				// Manager doesn't accept DMs or bot is blocked
			}
		}

		private static async Task WarnPlayerAsync(IGuild guild, Player player, Competition dbCompetition)
		{
			// Gets user
			IGuildUser? user = await guild.GetUserAsync(player.Id);

			if (user == null) return;

			// Checks that Clash of Clans API is online
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await WarnPlayerApiOfflineAsync(user, player, dbCompetition);

				return;
			}

			// Checks if player war status is out
			ClashOfClans.Models.Player cPlayer = player.Account;
			bool isOut = cPlayer.WarPreference == ClashOfClans.Models.WarPreference.Out;

			if (isOut) return;

			// Creates button
			ButtonBuilder playerBuilder = new ButtonBuilder()
				.WithLabel(cPlayer.Name)
				.WithStyle(ButtonStyle.Link)
				.WithUrl(cPlayer.GetLink(dbCompetition.Guild.Language.GetShortcutValue()));

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithButton(playerBuilder);

			// Gets text
			string warnText = dbCompetition.Guild.TimeText.WarRemindStatusWarnPlayer(cPlayer.Name, dbCompetition.Name);

			try
			{
				await user.SendMessageAsync(warnText, components: componentBuilder.Build());
			}
			catch (Exception)
			{
				// User doesn't accept DMs or bot is blocked
			}
		}
	}
}
