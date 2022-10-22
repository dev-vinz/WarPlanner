using System;
using System.Collections.Generic;

namespace Wp.Database.EFModels
{
    public partial class Guild
    {
        public Guild()
        {
            Calendars = new HashSet<Calendar>();
            Clans = new HashSet<Clan>();
            Competitions = new HashSet<Competition>();
            PlayerStatistics = new HashSet<PlayerStatistic>();
            Players = new HashSet<Player>();
            Roles = new HashSet<Role>();
            Times = new HashSet<Time>();
            WarStatistics = new HashSet<WarStatistic>();
        }

        public decimal Id { get; set; }
        public int Language { get; set; }
        public int TimeZone { get; set; }
        public int PremiumLevel { get; set; }
        public int MinThlevel { get; set; }

        public virtual ICollection<Calendar> Calendars { get; set; }
        public virtual ICollection<Clan> Clans { get; set; }
        public virtual ICollection<Competition> Competitions { get; set; }
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Time> Times { get; set; }
        public virtual ICollection<WarStatistic> WarStatistics { get; set; }
    }
}
