using System;
using System.Collections.Generic;

namespace Wp.Database.EFModels
{
    public partial class Competition
    {
        public decimal Guild { get; set; }
        public decimal CategoryId { get; set; }
        public decimal ResultId { get; set; }
        public string Name { get; set; } = null!;
        public string MainClan { get; set; } = null!;
        public string? SecondClan { get; set; }

        public virtual Guild GuildNavigation { get; set; } = null!;
    }
}
