using System;
using System.Collections.Generic;

namespace Wp.Database.EFModels;

public partial class Player
{
    public decimal Guild { get; set; }

    public decimal DiscordId { get; set; }

    public string Tag { get; set; } = null!;

    public virtual Guild GuildNavigation { get; set; } = null!;
}
