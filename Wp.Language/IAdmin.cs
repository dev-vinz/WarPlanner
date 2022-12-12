﻿namespace Wp.Language
{
	public interface IAdmin
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           GLOBAL CALENDAR                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdNotSet { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      CALENDAR CHANNEL COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarWillBeDisplayedHere(string user);
		public string CalendarChannelChanged(string channel);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      CALENDAR OPTIONS COMMAND                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionsDisplayed(bool enabled);
		public string CalendarOptionsFrequency(string? frequency);
		public string CalendarOptionsRemind(bool enabled);
		public string CalendarOptionsChooseRemind(string? reminds);
		public string CalendarChooseOption { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                   CALENDAR OPTIONS DISPLAY BUTTON                 *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionChanneNotSet { get; }
		public string CalendarOptionsDisplayEnabled(string channel);
		public string CalendarOptionsDisplayDisabled { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*             CALENDAR OPTIONS DISPLAY FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionDisplayNotEnabled { get; }
		public string CalendarOptionDisplayFrequencyPerDayLabel(int option);
		public string CalendarOptionDisplayFrequencyPerDayDescription(int option);
		public string CalendarOptionDisplayFrequencyChoose { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*             CALENDAR OPTIONS DISPLAY FREQUENCY SELECT             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionDisplayFrequencyUpdated(string nbPerDay);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdAdded(string id);
		public string CalendarIdUpdated(string id);
	}
}