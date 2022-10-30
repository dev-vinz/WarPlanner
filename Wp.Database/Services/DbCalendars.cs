using Wp.Common.Models;
using Wp.Database.Services.Extensions;
using Context = Wp.Database.EFModels.HEARC_P3Context;

namespace Wp.Database.Services
{
	public class DbCalendars : List<Calendar>
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static readonly object _lock = new();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public DbCalendars(IEnumerable<Calendar> calendars)
		{
			calendars.ForEach(c => base.Add(c));
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public new void Add(Calendar calendar)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.Calendars.Add(calendar.ToEFModel());
				ctx.SaveChanges();
			}

			base.Add(calendar);
		}

		public new bool Remove(Calendar calendar)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Calendar dbCalendar = ctx.Calendars.GetEFModel(calendar);
				ctx.Calendars.Remove(dbCalendar);
				ctx.SaveChanges();
			}

			return base.Remove(calendar);
		}

		public bool Remove(Predicate<Calendar> predicate)
		{
			Calendar? calendar = Find(predicate);

			return calendar != null && Remove(calendar);
		}

		public void Update(Calendar calendar)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Calendar dbCalendar = ctx.Calendars.GetEFModel(calendar);

				dbCalendar.CalendarId = calendar.Id;
				dbCalendar.ChannelId = calendar.ChannelId;
				dbCalendar.MessageId = calendar.MessageId;

				ctx.Calendars.Update(dbCalendar);
				ctx.SaveChanges();
			}

			base.Remove(calendar);
			base.Add(calendar);
		}
	}
}
