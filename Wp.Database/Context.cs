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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Calendar[] calendars = ctx.Calendars
                        .Include(c => c.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbCalendars(calendars);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Clan[] clans = ctx.Clans
                        .Include(c => c.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbClans(clans);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Competition[] competitions = ctx.Competitions
                        .Include(c => c.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbCompetitions(competitions);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Guild[] guilds = ctx.Guilds
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbGuilds(guilds);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Player[] players = ctx.Players
                        .Include(p => p.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbPlayers(players);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.PlayerStatistic[] playerStatistics = ctx.PlayerStatistics
                        .Include(ps => ps.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbPlayerStatistics(playerStatistics);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Role[] roles = ctx.Roles
                        .Include(r => r.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbRoles(roles);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.Time[] times = ctx.Times
                        .Include(t => t.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbTimes(times);
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
                    EFModels.HEARC_P3Context ctx = new();

                    Common.Models.WarStatistic[] warStatistics = ctx.WarStatistics
                        .Include(ws => ws.GuildNavigation)
                        .Select(c => c.ToModel())
                        .CopyAsArray();

                    ctx.Dispose();

                    return new DbWarStatistics(warStatistics);
                }
            }
        }
    }
}
