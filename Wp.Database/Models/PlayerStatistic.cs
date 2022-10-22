using System;
using System.Collections.Generic;

namespace Wp.Database.Models
{
    public partial class PlayerStatistic
    {
        public decimal Guild { get; set; }
        public decimal DiscordId { get; set; }
        public string PlayerTag { get; set; } = null!;
        public string ClanTag { get; set; } = null!;
        public DateTime WarDateStart { get; set; }
        public int AttackOrder { get; set; }
        public int WarType { get; set; }
        public int StatisticType { get; set; }
        public int StatisticAction { get; set; }
        public int Stars { get; set; }
        public int Percentage { get; set; }
        public int Duration { get; set; }

        public virtual Guild GuildNavigation { get; set; } = null!;
    }
}
