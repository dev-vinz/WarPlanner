namespace Wp.Language
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

		public string CalendarOptionChannelNotSet { get; }
		public string CalendarOptionDisplayEnabled(string channel);
		public string CalendarOptionDisplayDisabled { get; }

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
		|*                   CALENDAR OPTIONS REMIND BUTTON                  *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionRemindEnabled { get; }
		public string CalendarOptionRemindDisabled { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*              CALENDAR OPTIONS REMIND FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionRemindNotEnabled { get; }
		public string CalendarOptionRemindFrequencyLabel(int option);
		public string CalendarOptionRemindFrequencyChoose { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*              CALENDAR OPTIONS REMIND FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionRemindFrequencyUpdated(int[] options);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdAdded(string id);
		public string CalendarIdUpdated(string id);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      ROLE MANAGER ADD COMMAND                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RoleManagerAddTooManyRoles(int nb);
		public string RoleManagerAddRoleAlreadyExists { get; }
		public string RoleManagerAddRoleAdded { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    ROLE MANAGER DELETE COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RoleManagerDeleteNoRoles { get; }
		public string RoleManagerDeleteSelectRole { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    ROLE MANAGER DELETE SELECT                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RoleManagerDeleteSelectRoleDeleted { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      ROLE PLAYER ADD COMMAND                      *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RolePlayerAddTooManyRoles(int nb);
		public string RolePlayerAddRoleAlreadyExists { get; }
		public string RolePlayerAddRoleAdded { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                     ROLE PLAYER DELETE COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RolePlayerDeleteNoRoles { get; }
		public string RolePlayerDeleteSelectRole { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                     ROLE PLAYER DELETE SELECT                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RolePlayerDeleteSelectRoleDeleted { get; }
	}
}
