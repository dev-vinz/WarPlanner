using Discord;
using System.Globalization;
using Wp.Api;
using Wp.Api.Extensions;
using Wp.Api.Models;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database;
using Wp.Database.Services;
using Wp.Database.Settings;

namespace Wp.Bot.Modules.TimeEvents.War
{
	public static class Remind
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
				.FirstOrDefault(t => t.Guild == dbGuild && t.Action == TimeAction.REMIND_WAR);

			// Makes some verifications
			if (dbCalendar == null || dbTime == null) return;

			if (!dbTime.IsScanAllowed()) return;

			// Update time
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;

			dbTime.Date = utcNow;
			times.Update(dbTime);

			// Gets remind informations
			int[] reminders = dbTime.Optional?
				.ToCharArray()?
				.Select(c => int.Parse(c.ToString()))?
				.Select(i => Settings.CALENDAR_REMIND_WAR.GetValueOrDefault(i))?
				.Where(r => r != 0)?
				.ToArray() ?? Array.Empty<int>();

			// Gets all events
			CalendarEvent[] calendarEvents = await GoogleCalendarApi.Events.ListAsync(dbCalendar.Id);

			// Filters events and warn players
			calendarEvents
				.AsParallel()
				.Where(e =>
				{
					TimeSpan timeSpan = e.Start - e.End;
					int nbMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);
					return reminders.Any(r => r == nbMinutes);
				})
				.Where(e => dbCompetitions.Any(c => c.Name == e.CompetitionName))
				.ForAll(@event =>
				{
					Competition competition = dbCompetitions.First(c => c.Name == @event.CompetitionName);

					@event.Players
						.AsParallel()
						.Select(tag => dbPlayers.FirstOrDefault(p => p.Tag == tag))
						.Where(p => p != null)
						.ForAll(async player => await WarnPlayerAsync(guild, player!, @event, competition));
				});
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static async Task WarnApiOffline(IGuildUser user, Player player, CalendarEvent calendarEvent)
		{
			// Gets remaining time
			TimeSpan remaining = calendarEvent.Start.UtcDateTime - DateTime.UtcNow;
			DateTime dateRemaining = new(remaining.Ticks, DateTimeKind.Utc);

			// Gets text
			CultureInfo cultureInfo = player.Guild.CultureInfo;
			string warnText = player.Guild.TimeText.WarRemindApiOffline(user.DisplayName, calendarEvent.CompetitionName, dateRemaining.ToString("HH:mm", cultureInfo));

			await user.SendMessageAsync(warnText);
		}

		private static async Task WarnPlayerAsync(IGuild guild, Player player, CalendarEvent calendarEvent, Competition dbCompetition)
		{
			// Gets user
			IGuildUser? user = await guild.GetUserAsync(player.Id);

			if (user == null) return;

			// Checks that Clash of Clans API is online
			if (!await ClashOfClansApi.TryAccessApiAsync())
			{
				await WarnApiOffline(user, player, calendarEvent);

				return;
			}

			// Gets clan war league group
			ClashOfClans.Models.ClanWarLeagueGroup? mainLeagueGroup = await ClashOfClansApi.Clans.GetWarLeagueGroupAsync(dbCompetition.MainTag);

			// Checks if a clan war league is active, and if a second clan exists
			bool isMainInLeague = mainLeagueGroup != null && mainLeagueGroup.State != ClashOfClans.Models.State.Ended;
			bool isSecond = dbCompetition.SecondClan != null;

			// Gets clan in function
			ClashOfClans.Models.Clan cClan = isMainInLeague && isSecond ? dbCompetition.SecondClan! : dbCompetition.MainClan;

			// Checks if members is already in clan
			if (cClan.MemberList?.Any(m => m.Tag == player.Tag) ?? false) return;

			// Gets remaining time
			TimeSpan remaining = calendarEvent.Start.UtcDateTime - DateTime.UtcNow;
			DateTime dateRemaining = new(remaining.Ticks, DateTimeKind.Utc);

			// Creates button
			ButtonBuilder clanBuilder = new ButtonBuilder()
				.WithLabel(cClan.Name)
				.WithStyle(ButtonStyle.Link)
				.WithUrl(cClan.GetLink(player.Guild.Language.GetShortcutValue()));

			ButtonBuilder opponentBuilder = new ButtonBuilder()
				.WithLabel(calendarEvent.OpponentClan.Name)
				.WithStyle(ButtonStyle.Link)
				.WithUrl(calendarEvent.OpponentClan.GetLink(player.Guild.Language.GetShortcutValue()));

			// Build component
			ComponentBuilder componentBuilder = new ComponentBuilder()
				.WithButton(clanBuilder)
				.WithButton(opponentBuilder);

			// Gets text
			CultureInfo cultureInfo = player.Guild.CultureInfo;
			string warnText = player.Guild.TimeText.WarRemindWarnPlayer(player.Account.Name, cClan.Name, dbCompetition.Name, calendarEvent.OpponentClan.Name, calendarEvent.Start.ToString("HH:mm", cultureInfo), dateRemaining.ToString("HH:mm", cultureInfo));

			await user.SendMessageAsync(warnText, components: componentBuilder.Build());
		}
	}
}
