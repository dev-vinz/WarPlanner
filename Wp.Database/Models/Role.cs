using System;
using System.Collections.Generic;

namespace Wp.Database.Models
{
    public partial class Role
    {
        public decimal Guild { get; set; }
        public decimal Id { get; set; }
        public int Type { get; set; }

        public virtual Guild GuildNavigation { get; set; } = null!;
    }
}
