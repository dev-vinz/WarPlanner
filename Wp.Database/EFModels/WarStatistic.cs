using System;
using System.Collections.Generic;

namespace Wp.Database.EFModels;

public partial class WarStatistic
{
    public decimal Guild { get; set; }

    public DateTime DateStart { get; set; }

    public int WarType { get; set; }

    public string ClanTag { get; set; } = null!;

    public decimal? CompetitionCategory { get; set; }

    public string OpponentName { get; set; } = null!;

    public int Result { get; set; }

    public int AttackStars { get; set; }

    public decimal AttackPercent { get; set; }

    public decimal AttackAvgDuration { get; set; }

    public int DefenseStars { get; set; }

    public decimal DefensePercent { get; set; }

    public decimal DefenseAvgDuration { get; set; }

    public virtual Guild GuildNavigation { get; set; } = null!;
}
