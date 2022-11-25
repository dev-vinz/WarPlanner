using Microsoft.EntityFrameworkCore;
using Wp.Database.Services;
using Wp.Database.Services.Extensions;

namespace Wp.Database
{
    public static class Context
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static readonly object _lock = new();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets calendars registered inside the database
        /// </summary>
        public static DbCalendars Calendars
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Calendar[] calendars = ctx.Calendars
                            .Include(c => c.GuildNavigation)
                            .AsParallel()
                            .Select(c => c.ToModel())
                            .ToArray();

                    return new DbCalendars(calendars.Copy());
                }
            }
        }

        /// <summary>
        /// Gets clans registered inside the database
        /// </summary>
        public static DbClans Clans
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Clan[] clans = ctx.Clans
                            .Include(c => c.GuildNavigation)
                            .AsParallel()
                            .Select(c => c.ToModel())
                            .ToArray();

                    return new DbClans(clans.Copy());
                }
            }
        }

        /// <summary>
        /// Gets competitions registered inside the database
        /// </summary>
        public static DbCompetitions Competitions
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Competition[] competitions = ctx.Competitions
                            .Include(c => c.GuildNavigation)
                            .AsParallel()
                            .Select(c => c.ToModel())
                            .ToArray();

                    return new DbCompetitions(competitions.Copy());
                }
            }
        }

        /// <summary>
        /// Gets guilds registered inside the database
        /// </summary>
        public static DbGuilds Guilds
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Guild[] guilds = ctx.Guilds
                            .AsParallel()
                            .Select(g => g.ToModel())
                            .ToArray();

                    return new DbGuilds(guilds.Copy());
                }
            }
        }

        /// <summary>
        /// Gets players registered inside the database
        /// </summary>
        public static DbPlayers Players
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Player[] players = ctx.Players
                            .Include(p => p.GuildNavigation)
                            .AsParallel()
                            .Select(p => p.ToModel())
                            .ToArray();

                    return new DbPlayers(players.Copy());
                }
            }
        }

        /// <summary>
        /// Gets player' statistics registered inside the database
        /// </summary>
        public static DbPlayerStatistics PlayerStatistics
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.PlayerStatistic[] playerStatistics = ctx.PlayerStatistics
                            .Include(ps => ps.GuildNavigation)
                            .AsParallel()
                            .Select(ps => ps.ToModel())
                            .ToArray();

                    return new DbPlayerStatistics(playerStatistics.Copy());
                }
            }
        }

        /// <summary>
        /// Gets roles registered inside the database
        /// </summary>
        public static DbRoles Roles
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Role[] roles = ctx.Roles
                            .Include(r => r.GuildNavigation)
                            .AsParallel()
                            .Select(r => r.ToModel())
                            .ToArray();

                    return new DbRoles(roles.Copy());
                }
            }
        }

        /// <summary>
        /// Gets times registered inside the database
        /// </summary>
        public static DbTimes Times
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Time[] times = ctx.Times
                            .Include(t => t.GuildNavigation)
                            .AsParallel()
                            .Select(t => t.ToModel())
                            .ToArray();

                    return new DbTimes(times.Copy());
                }
            }
        }

        /// <summary>
        /// Gets wars' statistics registered inside the database
        /// </summary>
        public static DbWarStatistics WarStatistics
        {
            get
            {
                lock (_lock)
                {
                    using EFModels.HEARC_P3Context ctx = new();

                    Common.Models.WarStatistic[] warStatistics =
                        ctx.WarStatistics
                            .Include(ws => ws.GuildNavigation)
                            .AsParallel()
                            .Select(ws => ws.ToModel())
                            .ToArray();

                    return new DbWarStatistics(warStatistics.Copy());
                }
            }
        }
    }
}
