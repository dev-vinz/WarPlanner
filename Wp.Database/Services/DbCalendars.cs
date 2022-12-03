using Wp.Common.Models;

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

		public DbCalendars(Calendar[] calendars)
		{
			lock (_lock)
			{
				calendars
					.ToList()
					.ForEach(c => base.Add(c));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Adds a calendar to the database and saves it
		/// </summary>
		/// <param name="calendar">The calendar to be added to the database</param>
		public new void Add(Calendar calendar)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				ctx.Calendars.Add(calendar.ToEFModel());
				ctx.SaveChanges();

				base.Add(calendar);
			}
		}

		/// <summary>
		/// Removes a calendar from the database
		/// </summary>
		/// <param name="calendar">The calendar to be removed</param>
		/// <returns>true if calendar is successfully removed; false otherwise</returns>
		public new bool Remove(Calendar calendar)
		{
			lock (_lock)
			{
				using EFModels.HEARC_P3Context ctx = new();

				EFModels.Calendar dbCalendar = ctx.Calendars.GetEFModel(calendar);
				ctx.Calendars.Remove(dbCalendar);
				ctx.SaveChanges();

				return base.Remove(calendar);
			}
		}

		/// <summary>
		/// Removes a calendar from the database
		/// </summary>
		/// <param name="predicate">A delegate for the matching calendar to be removed</param>
		/// <returns>true if calendar is successfully removed; false otherwise</returns>
		public bool Remove(Predicate<Calendar> predicate)
		{
			Calendar? calendar = Find(predicate);

			return calendar != null && Remove(calendar);
		}

		/// <summary>
		/// Updates a calendar in the database
		/// </summary>
		/// <param name="calendar">The calendar to be updated</param>
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

				base.Remove(calendar);
				base.Add(calendar);
			}
		}
	}
}
