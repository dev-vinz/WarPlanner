using System;
using System.Collections.Generic;

namespace Wp.Database.Models
{
    public partial class Time
    {
        public decimal Guild { get; set; }
        public int Action { get; set; }
        public DateTime Date { get; set; }
        public int Interval { get; set; }
        public string Additional { get; set; } = null!;
        public string? Optional { get; set; }

        public virtual Guild GuildNavigation { get; set; } = null!;
    }
}
