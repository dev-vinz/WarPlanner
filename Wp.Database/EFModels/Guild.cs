using System;
using System.Collections.Generic;

namespace Wp.Database.EFModels;

public partial class Guild
{
    public decimal Id { get; set; }

    public int Language { get; set; }

    public int TimeZone { get; set; }

    public int PremiumLevel { get; set; }

    public int MinThlevel { get; set; }

    public virtual Calendar? Calendar { get; set; }

    public virtual ICollection<Clan> Clans { get; } = new List<Clan>();

    public virtual ICollection<Competition> Competitions { get; } = new List<Competition>();

    public virtual ICollection<PlayerStatistic> PlayerStatistics { get; } = new List<PlayerStatistic>();

    public virtual ICollection<Player> Players { get; } = new List<Player>();

    public virtual ICollection<Role> Roles { get; } = new List<Role>();

    public virtual ICollection<Time> Times { get; } = new List<Time>();

    public virtual ICollection<WarStatistic> WarStatistics { get; } = new List<WarStatistic>();
}
