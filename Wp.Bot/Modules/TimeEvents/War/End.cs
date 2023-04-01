using Discord;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Wp.Api;
using Wp.Api.Extensions;
using Wp.Api.Models;
using Wp.Common.Models;
using Wp.Common.Settings;
using Wp.Database;
using Wp.Database.Services;
using Wp.Database.Settings;
using Wp.Discord;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;

namespace Wp.Bot.Modules.TimeEvents.War
{
	public static class End
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly int MINUTES_DELAY_ACCEPTED = 15;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static void Execute(IGuild guild)
		{
			// Loads databases infos
			DbClans clans = Context.Clans;
			DbTimes times = Context.Times;
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == guild.Id);

			// Filters for guild
			Clan[] dbClans = clans
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			Time? dbTime = times
				.AsParallel()
				.FirstOrDefault(t => t.Guild == dbGuild && t.Action == TimeAction.DETECT_END_WAR);

			// Makes some verifications
			if (dbTime == null) return;

			if (!dbTime.IsScanAllowed()) return;

			// Update time
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;

			dbTime.Date = utcNow;
			times.Update(dbTime);

			// Scan all clans
			dbClans
				.AsParallel()
				.Select(async c => await ClashOfClansApi.Clans.GetCurrentWarAsync(c.Tag))
				.Select(task => task.Result)
				.Where(war => war != null && war.State == ClashOfClans.Models.State.WarEnded)
				.ForAll(async war => await ScanCurrentWarAsync(guild, war!));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static async Task DisplayResultAsync(IGuild guild, Competition? competition, ClashOfClans.Models.ClanWar cWar)
		{
			if (competition == null) return;

			// Gets result channel
			ITextChannel channel = await guild.GetTextChannelAsync(competition.ResultId);

			if (channel == null) return;

			// Gets main image
			List<FileAttachment> attachments = new()
			{
				await GetMainResult(cWar, competition.Guild)
			};

			// Gets additional images
			int nbPages = (int)Math.Ceiling((cWar.TeamSize - 5) / 10.0);
			for (int k = 0; k < nbPages; k++) attachments.Add(await GetAdditionnalResult(cWar, k));

			attachments.ForEach(async attachment =>
			{
				try
				{
					await channel.SendFileAsync(attachment);
				}
				catch (Exception)
				{
					// Bot can't send message into the channel, do nothing
				}
			});
		}

		private static async Task<(bool isEsport, CalendarEvent? match)> GetEsportInformationsAsync(ulong guildId, ClashOfClans.Models.ClanWar cWar)
		{
			// Makes some verifications
			TimeSpan preparationTime = cWar.StartTime - cWar.PreparationStartTime;

			if (preparationTime.TotalHours == DefaultParameters.DEFAULT_HOUR_WAR_PREPARATION_TIME) return (false, null);

			Common.Models.Calendar? dbCalendar = Context.Calendars
				.FirstOrDefault(c => c.Guild.Id == guildId);

			Competition[] dbCompetitions = Context.Competitions
				.Where(c => c.Guild.Id == guildId)
				.ToArray();

			if (dbCalendar == null) return (false, null);

			DateTimeOffset startWar = new(cWar.PreparationStartTime.ToUniversalTime(), TimeZoneInfo.Utc.BaseUtcOffset);
			DateTimeOffset endWar = new(cWar.EndTime.ToUniversalTime(), TimeZoneInfo.Utc.BaseUtcOffset);

			IEnumerable<CalendarEvent> allEvents = await GoogleCalendarApi.Events.ListAsync(dbCalendar.Id, false);

			// e.StartWar € [startWar - 15; startWar + 15]
			// startWar - 15 <= e.StartWar <= startWar + 15
			IEnumerable<CalendarEvent> matchStart = allEvents.Where(e => startWar.AddMinutes(-MINUTES_DELAY_ACCEPTED) <= e.Start.UtcDateTime && e.Start.UtcDateTime <= startWar.AddMinutes(MINUTES_DELAY_ACCEPTED));

			// e.EndWar € [endWar - 15; endWar + 15]
			// endWar - 15 <= e.EndWar <= endWar + 15
			IEnumerable<CalendarEvent> matchEnd = matchStart.Where(e => endWar.AddMinutes(-MINUTES_DELAY_ACCEPTED) <= e.End.UtcDateTime && e.End.UtcDateTime <= endWar.AddMinutes(MINUTES_DELAY_ACCEPTED));

			if (!matchEnd.Any()) return (false, null);

			// Check if the tournament is still in database and clan is valid
			IEnumerable<CalendarEvent> matchTournament = matchEnd
				.AsParallel()
				.Where(e => dbCompetitions.Any(c => c.Name == e.CompetitionName && (c.MainTag == cWar.Clan.Tag || c.SecondTag == cWar.Clan.Tag)));

			if (!matchTournament.Any()) return (false, null);

			return (true, matchTournament.First());
		}

		private static async Task<FileAttachment> GetAdditionnalResult(ClashOfClans.Models.ClanWar cWar, int index)
		{
			// Gets image
			HttpClient client = new();
			using Stream bytes = await client.GetStreamAsync(Utilities.SECOND_BACKGROUND_WAR_RESULT);
			Image image = Image.Load(bytes, new PngDecoder());

			// Initializes some settings
			IPen blackPen = new Pen(Color.Black, 1);
			WarResult result = GetWarResult(cWar);

			Color color = Color.FromRgb(41, 50, 46);

			if (!SystemFonts.Collection.TryGet("Quicksand SemiBold", out FontFamily fontFamily))
			{
				fontFamily = SystemFonts.Collection.Families.FirstOrDefault();
			}

			// Upgrade image
			image.Mutate(ctx =>
			{
				// Gets first 5 players of each clan
				(ClashOfClans.Models.ClanWarMember First, ClashOfClans.Models.ClanWarMember Second)[] players = cWar.Clan.Members?
					.OrderBy(m => m.MapPosition)
					.Skip(index * 10 + 5)
					.Take(10)
					.Zip(cWar.Opponent.Members?.OrderBy(m => m.MapPosition).Skip(index * 10 + 5).Take(10) ?? Enumerable.Empty<ClashOfClans.Models.ClanWarMember>())
					.ToArray()!;

				// Draw players
				players
					.AsParallel()
					.ForAll(tuple =>
					{
						// Gets players
						ClashOfClans.Models.ClanWarMember clanPlayer = tuple.First;
						ClashOfClans.Models.ClanWarMember opponentPlayer = tuple.Second;

						// Some settings
						int factor = clanPlayer.MapPosition - (index * 10 + 6);
						Font font = fontFamily.CreateFont(20, FontStyle.Bold);

						// Draw names
						ctx.DrawText(clanPlayer.Name, font, Color.White, new(50, 80 + factor * 60));
						ctx.DrawText(new TextOptions(font) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(1235, 80 + factor * 60) }, opponentPlayer.Name, Color.White);

						// Draw stars
						Task<Stream> starEmote = client.GetStreamAsync(CustomEmojis.CocStar.Url);
						starEmote.Wait();

						using Stream starBytes = starEmote.Result;

						Image starImage = Image.Load(starBytes, new PngDecoder());
						starImage.Mutate(ctx => ctx.Resize(25, 25));

						int clanStars = clanPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.Stars) ?? 0;
						for (int k = 0; k < clanStars; k++) ctx.DrawImage(starImage, new Point(265 + k * 25, 80 + factor * 60), 1);

						int opponentStars = opponentPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.Stars) ?? 0;
						for (int k = 0; k < opponentStars; k++) ctx.DrawImage(starImage, new Point(995 - k * 25, 80 + factor * 60), 1);

						// Draw percent
						ctx.DrawText($"{clanPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.DestructionPercentage):#0.00}%", font, Color.White, new(460, 80 + factor * 60));
						ctx.DrawText(new TextOptions(font) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(825, 80 + factor * 60) }, $"{opponentPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.DestructionPercentage):#0.00}%", Color.White);
					});
			});

			// Saves and returns image
			MemoryStream ms = new();
			image.SaveAsPng(ms);

			return new(ms, "war_result.png");
		}

		private static async Task<FileAttachment> GetMainResult(ClashOfClans.Models.ClanWar cWar, Guild guild)
		{
			// Gets image
			HttpClient client = new();
			using Stream bytes = await client.GetStreamAsync(Utilities.MAIN_BACKGROUND_WAR_RESULT);
			Image image = Image.Load(bytes, new PngDecoder());

			// Initializes some settings
			IPen blackPen = new Pen(Color.Black, 1);
			WarResult result = GetWarResult(cWar);

			Color color = Color.FromRgb(41, 50, 46);

			if (!SystemFonts.Collection.TryGet("Quicksand SemiBold", out FontFamily fontFamily))
			{
				fontFamily = SystemFonts.Collection.Families.FirstOrDefault();
			}

			// Upgrade image
			image.Mutate(ctx =>
			{
				// Gets badges images
				Task<Stream> clanTask = client.GetStreamAsync(cWar.Clan.BadgeUrls.Medium);
				Task<Stream> opponentTask = client.GetStreamAsync(cWar.Opponent.BadgeUrls.Medium);
				Task<Stream> crown = client.GetStreamAsync(Utilities.WAR_RESULT_CROWN);

				clanTask.Wait();
				opponentTask.Wait();
				crown.Wait();

				using Stream clanBytes = clanTask.Result;
				using Stream opponentBytes = opponentTask.Result;
				using Stream crownBytes = crown.Result;

				// Draw images
				Image clanBadge = Image.Load(clanBytes, new PngDecoder());
				Image opponentBadge = Image.Load(opponentBytes, new PngDecoder());

				ctx.DrawImage(clanBadge, new Point(115, 75), 1);
				ctx.DrawImage(opponentBadge, new Point(969, 75), 1);

				// Draw crown on winner
				WarResult result = GetWarResult(cWar);

				int rotateFactor = result == WarResult.WIN ? -1 : 1;
				Point position = result == WarResult.WIN ? new Point(75, -5) : new Point(1025, -5);

				Image crownImg = Image.Load(crownBytes, new PngDecoder());
				crownImg.Mutate(ctx => { ctx.Resize(150, 100); ctx.Rotate(rotateFactor * 30); });

				ctx.DrawImage(crownImg, position, 1);

				// Draw team names
				Font fontClans = fontFamily.CreateFont(30, FontStyle.Bold);

				ctx.DrawText(cWar.Clan.Name!, fontClans, color, new(305, 110));
				ctx.DrawText(new TextOptions(fontClans) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(975, 110) }, cWar.Opponent.Name!, color);

				// Draw war informations
				Font fontInfos = fontFamily.CreateFont(30, FontStyle.Bold);

				TimeSpan clanDuration = cWar.GetAverageAttackTime();
				TimeSpan opponentDuration = cWar.GetAverageDefenseTime();

				ctx.DrawText($"{cWar.Clan.DestructionPercentage:#0.00}%", fontInfos, color, new(305, 165));
				ctx.DrawText($"{clanDuration:%m}{guild.GeneralResponses.ShortcutMinute} {clanDuration:ss}{guild.GeneralResponses.ShortcutSecond}", fontInfos, color, new(305, 195));

				ctx.DrawText(new TextOptions(fontInfos) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(975, 165) }, $"{cWar.Opponent.DestructionPercentage:#0.00}%", color);
				ctx.DrawText(new TextOptions(fontInfos) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(975, 195) }, $"{opponentDuration:%m}{guild.GeneralResponses.ShortcutMinute} {opponentDuration:ss}{guild.GeneralResponses.ShortcutSecond}", color);

				// Draw stars
				Font fontStars = fontFamily.CreateFont(70, FontStyle.Bold);

				ctx.DrawText(new TextOptions(fontStars) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(590, 165) }, $"{cWar.Clan.Stars}", color);
				ctx.DrawText($"{cWar.Opponent.Stars}", fontStars, color, new(700, 165));

				// Gets first 5 players of each clan
				(ClashOfClans.Models.ClanWarMember First, ClashOfClans.Models.ClanWarMember Second)[] players = cWar.Clan.Members?
					.OrderBy(m => m.MapPosition)
					.Take(5)
					.Zip(cWar.Opponent.Members?.OrderBy(m => m.MapPosition).Take(5) ?? Enumerable.Empty<ClashOfClans.Models.ClanWarMember>())
					.ToArray()!;

				// Draw players
				players
					.AsParallel()
					.ForAll(tuple =>
					{
						// Gets players
						ClashOfClans.Models.ClanWarMember clanPlayer = tuple.First;
						ClashOfClans.Models.ClanWarMember opponentPlayer = tuple.Second;

						// Some settings
						int factor = clanPlayer.MapPosition - 1;
						Font font = fontFamily.CreateFont(20, FontStyle.Bold);

						// Draw names
						ctx.DrawText(clanPlayer.Name, font, Color.White, new(50, 308 + factor * 60));
						ctx.DrawText(new TextOptions(font) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(1235, 308 + factor * 60) }, opponentPlayer.Name, Color.White);

						// Draw stars
						Task<Stream> starEmote = client.GetStreamAsync(CustomEmojis.CocStar.Url);
						starEmote.Wait();

						using Stream starBytes = starEmote.Result;

						Image starImage = Image.Load(starBytes, new PngDecoder());
						starImage.Mutate(ctx => ctx.Resize(25, 25));

						int clanStars = clanPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.Stars) ?? 0;
						for (int k = 0; k < clanStars; k++) ctx.DrawImage(starImage, new Point(265 + k * 25, 308 + factor * 60), 1);

						int opponentStars = opponentPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.Stars) ?? 0;
						for (int k = 0; k < opponentStars; k++) ctx.DrawImage(starImage, new Point(995 - k * 25, 308 + factor * 60), 1);

						// Draw percent
						ctx.DrawText($"{clanPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.DestructionPercentage):#0.00}%", font, Color.White, new(460, 308 + factor * 60));
						ctx.DrawText(new TextOptions(font) { HorizontalAlignment = HorizontalAlignment.Right, Origin = new(825, 308 + factor * 60) }, $"{opponentPlayer.Attacks?.Aggregate(0, (acc, a) => acc += a.DestructionPercentage):#0.00}%", Color.White);
					});
			});

			// Saves and returns image
			MemoryStream ms = new();
			image.SaveAsPng(ms);

			return new(ms, "war_result.png");
		}

		private static WarResult GetWarResult(ClashOfClans.Models.ClanWar war)
		{
			if (war.Clan.Stars > war.Opponent.Stars)
			{
				return WarResult.WIN;
			}
			else if (war.Clan.Stars == war.Opponent.Stars)
			{
				if (war.Clan.DestructionPercentage > war.Opponent.DestructionPercentage)
				{
					return WarResult.WIN;
				}
				else if (war.Clan.DestructionPercentage == war.Opponent.DestructionPercentage)
				{
					if (war.GetAverageAttackTime() < war.GetAverageDefenseTime())
					{
						return WarResult.WIN;
					}
					else if (war.GetAverageAttackTime() == war.GetAverageDefenseTime())
					{
						return WarResult.TIE;
					}
					else
					{
						return WarResult.LOOSE;
					}
				}
				else
				{
					return WarResult.LOOSE;
				}
			}
			else
			{
				return WarResult.LOOSE;
			}
		}

		private static void InsertPlayerStatistics(Guild guild, ClashOfClans.Models.ClanWar cWar, WarType warType)
		{
			// Loads database infos
			DbPlayers players = Context.Players;
			DbPlayerStatistics playerStatistics = Context.PlayerStatistics;

			DateTimeOffset startWar = new(cWar.PreparationStartTime, TimeZoneInfo.Utc.BaseUtcOffset);

			IReadOnlyCollection<ClashOfClans.Models.ClanWarAttack> allAttacks = cWar.GetAllAttacks();
			IReadOnlyCollection<ClashOfClans.Models.ClanWarAttack> allOpens = cWar.GetAllOpeningAttacks();

			cWar.Clan.Members?
				.AsParallel()
				.ForAll(m =>
				{
					Player? dbPlayer = players.FirstOrDefault(p => (p.Guild == guild || p.Guild.Id == Configurations.DEV_GUILD_ID) && p.Tag == m.Tag);

					if (dbPlayer is null) return;

					// Registers all attacks
					allAttacks
						.AsParallel()
						.Where(a => a.AttackerTag == m.Tag)
						.Select(a => new PlayerStatistic(guild, dbPlayer.Id, m.Tag, cWar.Clan.Tag!, startWar, a.Order, warType, PlayerStatisticType.ATTACK, allOpens.Contains(a) ? PlayerStatisticAction.OPENING : PlayerStatisticAction.RUNNING_OVER, a.Stars, a.DestructionPercentage, a.Duration))
						.ForAll(ps => playerStatistics.Add(ps));

					// Registers all defenses
					allAttacks
						.AsParallel()
						.Where(d => d.DefenderTag == m.Tag)
						.Select(d => new PlayerStatistic(guild, dbPlayer.Id, m.Tag, cWar.Clan.Tag!, startWar, d.Order, warType, PlayerStatisticType.DEFENSE, allOpens.Contains(d) ? PlayerStatisticAction.OPENING : PlayerStatisticAction.RUNNING_OVER, d.Stars, d.DestructionPercentage, d.Duration))
						.ForAll(ps => playerStatistics.Add(ps));
				});
		}

		private static async Task ScanCurrentWarAsync(IGuild guild, ClashOfClans.Models.ClanWar war)
		{
			// If the premium level is enough (>= Medium), keep the previous war statistics
			// Otherwise, delete all, and add only the last statistic

			// Loads database infos
			DbCompetitions competitions = Context.Competitions;
			DbWarStatistics warStatistics = Context.WarStatistics;
			Guild dbGuild = Context
				.Guilds
				.First(g => g.Id == guild.Id);

			// Filters for guild
			Competition[] dbCompetitions = competitions
				.AsParallel()
				.Where(c => c.Guild == dbGuild)
				.ToArray();

			WarStatistic[] dbWarStatistics = warStatistics
				.AsParallel()
				.Where(w => w.Guild == dbGuild)
				.OrderByDescending(w => w.Date)
				.ToArray();

			// Gets all esport informations
			DateTimeOffset startWar = new(war.PreparationStartTime, TimeZoneInfo.Utc.BaseUtcOffset);
			(bool isEsport, CalendarEvent? match) = await GetEsportInformationsAsync(guild.Id, war);
			Competition? competition = dbCompetitions.FirstOrDefault(c => c.Name == match?.CompetitionName);

			// Gets the last war added
			WarStatistic? lastWar = dbWarStatistics.FirstOrDefault(w => w.ClanTag == war.Clan.Tag);

			// Creates new one
			WarType warType = isEsport ? WarType.ESPORT : WarType.RANDOM;
			WarStatistic currentWar = new(dbGuild, startWar, warType, war.Clan.Tag ?? "", competition?.Id, war.Opponent.Name ?? "", GetWarResult(war), war.Clan.Stars, war.Clan.DestructionPercentage, war.GetAverageAttackTime().TotalSeconds, war.Opponent.Stars, war.Opponent.DestructionPercentage, war.GetAverageDefenseTime().TotalSeconds);

			// If the last war added is the current, do nothing
			if (lastWar == currentWar) return;

			// Adds the current to database
			warStatistics.Add(currentWar);

			// If premium isn't MEDIUM or higher, deletes old ones
			if (dbGuild.PremiumLevel < PremiumLevel.MEDIUM)
			{
				dbWarStatistics
				   .Where(w => w.ClanTag == war.Clan.Tag && w != currentWar)
				   .ToList()
				   .AsParallel()
				   .ForAll(w => warStatistics.Remove(w));
			}

			if (isEsport) await DisplayResultAsync(guild, competition, war);

			if (dbGuild.PremiumLevel >= PremiumLevel.MEDIUM && !isEsport) InsertPlayerStatistics(dbGuild, war, warType);
			if (dbGuild.PremiumLevel >= PremiumLevel.LOW && isEsport) InsertPlayerStatistics(dbGuild, war, warType);
		}
	}
}
