using Microsoft.EntityFrameworkCore;
using Wp.Common.Models;
using Wp.Common.Services.Extensions;

namespace Wp.Database
{
	public static class DatabaseExtensions
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              CALENDAR                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Calendar ToModel(this EFModels.Calendar calendar) => new(calendar.GuildNavigation.ToModel(), calendar.CalendarId)
		{
			ChannelId = (ulong?)calendar.ChannelId,
			MessageId = (ulong?)calendar.MessageId,
		};

		public static EFModels.Calendar ToEFModel(this Calendar calendar) => new()
		{
			Guild = calendar.Guild.Id,
			CalendarId = calendar.Id,
			ChannelId = calendar.ChannelId,
			MessageId = calendar.MessageId,
		};

		public static EFModels.Calendar GetEFModel(this DbSet<EFModels.Calendar> calendars, Calendar calendar)
		{
			return calendars.AsEnumerable().First(c => c.Guild == calendar.Guild.Id);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                                CLAN                               *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Clan ToModel(this EFModels.Clan clan) => new(clan.GuildNavigation.ToModel(), clan.Tag);

		public static EFModels.Clan ToEFModel(this Clan clan) => new()
		{
			Guild = clan.Guild.Id,
			Tag = clan.Tag,
		};

		public static EFModels.Clan GetEFModel(this DbSet<EFModels.Clan> clans, Clan clan)
		{
			return clans.AsEnumerable().First(c => c.Guild == clan.Guild.Id && c.Tag == clan.Tag);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            COMPETITION                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Competition ToModel(this EFModels.Competition competition) => new(competition.GuildNavigation.ToModel(), (ulong)competition.CategoryId, (ulong)competition.ResultId, competition.Name, competition.MainClan)
		{
			SecondTag = competition.SecondClan,
		};

		public static EFModels.Competition ToEFModel(this Competition competition) => new()
		{
			Guild = competition.Guild.Id,
			CategoryId = competition.Id,
			ResultId = competition.ResultId,
			Name = competition.Name,
			MainClan = competition.MainTag,
			SecondClan = competition.SecondTag,
		};

		public static EFModels.Competition GetEFModel(this DbSet<EFModels.Competition> competitions, Competition competition)
		{
			return competitions.AsEnumerable().First(c => c.Guild == competition.Guild.Id && c.CategoryId == competition.Id);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               GUILD                               *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Guild ToModel(this EFModels.Guild guild) => new((ulong)guild.Id, (Common.Models.TimeZone)guild.TimeZone)
		{
			Language = (Common.Models.Language)guild.Language,
			PremiumLevel = (PremiumLevel)guild.PremiumLevel,
			MinimalTownHallLevel = (uint)guild.MinThlevel,
		};

		public static EFModels.Guild ToEFModel(this Guild guild) => new()
		{
			Id = guild.Id,
			Language = (int)guild.Language,
			TimeZone = (int)guild.TimeZone,
			PremiumLevel = (int)guild.PremiumLevel,
			MinThlevel = (int)guild.MinimalTownHallLevel,
		};

		public static EFModels.Guild GetEFModel(this DbSet<EFModels.Guild> guilds, Guild guild)
		{
			return guilds.AsEnumerable().First(g => g.Id == guild.Id);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               PLAYER                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Player ToModel(this EFModels.Player player) => new(player.GuildNavigation.ToModel(), (ulong)player.DiscordId, player.Tag);

		public static EFModels.Player ToEFModel(this Player player) => new()
		{
			Guild = player.Guild.Id,
			DiscordId = player.Id,
			Tag = player.Tag,
		};

		public static EFModels.Player GetEFModel(this DbSet<EFModels.Player> players, Player player)
		{
			return players.AsEnumerable().First(p => p.Guild == player.Guild.Id && p.Tag == player.Tag);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PLAYER STATISTIC                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static PlayerStatistic ToModel(this EFModels.PlayerStatistic playerStatistic) => new(playerStatistic.GuildNavigation.ToModel(),
			(ulong)playerStatistic.DiscordId, playerStatistic.PlayerTag, playerStatistic.ClanTag,
			new DateTimeOffset(playerStatistic.WarDateStart, TimeZoneInfo.Utc.BaseUtcOffset), playerStatistic.AttackOrder,
			(WarType)playerStatistic.WarType, (PlayerStatisticType)playerStatistic.StatisticType, (PlayerStatisticAction)playerStatistic.StatisticAction,
			playerStatistic.Stars, playerStatistic.Percentage, playerStatistic.Duration);

		public static EFModels.PlayerStatistic ToEFModel(this PlayerStatistic playerStatistic) => new()
		{
			Guild = playerStatistic.Guild.Id,
			DiscordId = playerStatistic.PlayerId,
			PlayerTag = playerStatistic.PlayerTag,
			ClanTag = playerStatistic.ClanTag,
			WarDateStart = playerStatistic.Date.UtcDateTime,
			AttackOrder = playerStatistic.Order,
			WarType = (int)playerStatistic.WarType,
			StatisticType = (int)playerStatistic.Type,
			StatisticAction = (int)playerStatistic.Action,
			Stars = playerStatistic.Stars,
			Percentage = playerStatistic.Percent,
			Duration = playerStatistic.Duration,
		};

		public static EFModels.PlayerStatistic GetEFModel(this DbSet<EFModels.PlayerStatistic> playerStatistics, PlayerStatistic playerStatistic)
		{
			return playerStatistics.AsEnumerable().First(ps => ps.DiscordId == playerStatistic.PlayerId &&
															   ps.ClanTag == playerStatistic.ClanTag &&
															   DateTimeOffset.Compare(ps.WarDateStart, playerStatistic.Date) == 0 &&
															   ps.AttackOrder == playerStatistic.Order);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                                ROLE                               *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Role ToModel(this EFModels.Role role) => new(role.GuildNavigation.ToModel(), (ulong)role.Id, (RoleType)role.Type);

		public static EFModels.Role ToEFModel(this Role role) => new()
		{
			Guild = role.Guild.Id,
			Id = role.Id,
			Type = (int)role.Type,
		};

		public static EFModels.Role GetEFModel(this DbSet<EFModels.Role> roles, Role role)
		{
			return roles.AsEnumerable().First(r => r.Id == role.Id && r.Type == (int)role.Type);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                                TIME                               *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static Time ToModel(this EFModels.Time time) => new(time.GuildNavigation.ToModel(), (TimeAction)time.Action, new DateTimeOffset(time.Date, TimeZoneInfo.Utc.BaseUtcOffset), TimeSpan.FromSeconds(time.Interval), time.Additional)
		{
			Optional = time.Optional,
		};

		public static EFModels.Time ToEFModel(this Time time) => new()
		{
			Guild = time.Guild.Id,
			Action = (int)time.Action,
			Date = time.Date.UtcDateTime.TruncSeconds(),
			Interval = (int)time.Interval.TotalSeconds,
			Additional = time.Additional,
			Optional = time.Optional,
		};

		public static EFModels.Time GetEFModel(this DbSet<EFModels.Time> times, Time time)
		{
			return times.AsEnumerable().First(t => t.Guild == time.Guild.Id && t.Action == (int)time.Action && t.Additional == time.Additional);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           WAR STATISTIC                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static WarStatistic ToModel(this EFModels.WarStatistic warStatistic) => new(warStatistic.GuildNavigation.ToModel(),
			new DateTimeOffset(warStatistic.DateStart, TimeZoneInfo.Utc.BaseUtcOffset), (WarType)warStatistic.WarType, warStatistic.ClanTag,
			(ulong?)warStatistic.CompetitionCategory, warStatistic.OpponentName, (WarResult)warStatistic.Result,
			warStatistic.AttackStars, (double)warStatistic.AttackPercent, (double)warStatistic.AttackAvgDuration,
			warStatistic.DefenseStars, (double)warStatistic.DefensePercent, (double)warStatistic.DefenseAvgDuration);

		public static EFModels.WarStatistic ToEFModel(this WarStatistic warStatistic) => new()
		{
			Guild = warStatistic.Guild.Id,
			DateStart = warStatistic.Date.UtcDateTime,
			WarType = (int)warStatistic.Type,
			ClanTag = warStatistic.ClanTag,
			CompetitionCategory = warStatistic.CompetitionId,
			OpponentName = warStatistic.OpponentName,
			Result = (int)warStatistic.Result,
			AttackStars = warStatistic.AttackStars,
			AttackPercent = (decimal)warStatistic.AttackPercent,
			AttackAvgDuration = (decimal)warStatistic.AttackAvgDuration,
			DefenseStars = warStatistic.DefenseStars,
			DefensePercent = (decimal)warStatistic.DefensePercent,
			DefenseAvgDuration = (decimal)warStatistic.DefenseAvgDuration,
		};

		public static EFModels.WarStatistic GetEFModel(this DbSet<EFModels.WarStatistic> warStatistics, WarStatistic warStatistic)
		{
			return warStatistics.AsEnumerable().First(ws => DateTimeOffset.Compare(ws.DateStart, warStatistic.Date) == 0 && ws.ClanTag == warStatistic.ClanTag);
		}
	}
}
