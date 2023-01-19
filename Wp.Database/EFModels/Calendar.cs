using System;
using System.Collections.Generic;

namespace Wp.Database.EFModels;

public partial class Calendar
{
    public decimal Guild { get; set; }

    public string CalendarId { get; set; } = null!;

    public decimal? ChannelId { get; set; }

    public decimal? MessageId { get; set; }

    public virtual Guild GuildNavigation { get; set; } = null!;
}
