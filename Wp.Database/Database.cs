using Microsoft.EntityFrameworkCore;
using Wp.Database.Services;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

namespace Wp.Database
{
    public static class Database
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
                    using Context ctx = new();

                    return new DbCalendars(
                        ctx.Calendars
                            .Include(c => c.GuildNavigation)
                            .AsParallel()
                            .Select(c => c.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbClans(
                        ctx.Clans
                            .Include(c => c.GuildNavigation)
                            .AsParallel()
                            .Select(c => c.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbCompetitions(
                        ctx.Competitions
                            .Include(c => c.GuildNavigation)
                            .AsParallel()
                            .Select(c => c.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbGuilds(
                        ctx.Guilds
                            .AsParallel()
                            .Select(g => g.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbPlayers(
                        ctx.Players
                            .Include(p => p.GuildNavigation)
                            .AsParallel()
                            .Select(p => p.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbPlayerStatistics(
                        ctx.PlayerStatistics
                            .Include(ps => ps.GuildNavigation)
                            .AsParallel()
                            .Select(ps => ps.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbRoles(
                        ctx.Roles
                            .Include(r => r.GuildNavigation)
                            .AsParallel()
                            .Select(r => r.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbTimes(
                        ctx.Times
                            .Include(t => t.GuildNavigation)
                            .AsParallel()
                            .Select(t => t.ToModel())
                            .CopyAsArray()
                    );
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
                    using Context ctx = new();

                    return new DbWarStatistics(
                        ctx.WarStatistics
                            .Include(ws => ws.GuildNavigation)
                            .AsParallel()
                            .Select(ws => ws.ToModel())
                            .CopyAsArray()
                    );
                }
            }
        }
    }
}
