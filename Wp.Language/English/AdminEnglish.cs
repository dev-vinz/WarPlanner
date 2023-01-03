namespace Wp.Language.English
{
	public class AdminEnglish : IAdmin
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           GLOBAL CALENDAR                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdNotSet => "You have to set your calendar's ID";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                       ADMIN LANGUAGE COMMAND                      *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string AdminLanguageSelectChoose => "Choose the displayed language used by the bot";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                       ADMIN LANGUAGE SELECT                       *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string AdminLanguageChanged => "The display language has been changed to : **English**";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      CALENDAR CHANNEL COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarWillBeDisplayedHere(string user) => $"{user}, this is where I'll post the calendar\nDon't delete this message";
		public string CalendarChannelChanged(string channel) => $"Calendar will now appear in {channel}";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      CALENDAR OPTIONS COMMAND                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionsDisplayed(bool enabled) => enabled ? "Disable the display" : "Enable the display";
		public string CalendarOptionsFrequency(string? frequency) => $"Display frequency ({frequency ?? "0"})";
		public string CalendarOptionsRemind(bool enabled) => enabled ? "Disable reminders" : "Enable reminders";
		public string CalendarOptionsChooseRemind(string? reminds) => $"Numbers of reminders ({reminds ?? "0"})";
		public string CalendarChooseOption => "Click one of the buttons below to change the calendar settings";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                   CALENDAR OPTIONS DISPLAY BUTTON                 *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionChannelNotSet => $"Please first set up the display room using </calendar channel:1048881562772582440>";
		public string CalendarOptionDisplayEnabled(string channel) => $"Calendar view has been enabled in {channel}";
		public string CalendarOptionDisplayDisabled => "Calendar display has been disabled";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*             CALENDAR OPTIONS DISPLAY FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionDisplayNotEnabled => "Please enable calendar display first";
		public string CalendarOptionDisplayFrequencyPerDayLabel(int option) => $"{option} times a day";
		public string CalendarOptionDisplayFrequencyPerDayDescription(int option) => $"All {Math.Round(24.0 / option, 2)} hours";
		public string CalendarOptionDisplayFrequencyChoose => "Choose the frequency of the calendar display";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*             CALENDAR OPTIONS DISPLAY FREQUENCY SELECT             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionDisplayFrequencyUpdated(string nbPerDay) => $"Display frequency has been set to **{nbPerDay} times per day**";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                   CALENDAR OPTIONS REMIND BUTTON                  *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionRemindEnabled => "Match reminders has been activated. By default, the reminder is set to `2 hours before`";
		public string CalendarOptionRemindDisabled => "Match reminders to players has been disabled";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*              CALENDAR OPTIONS REMIND FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionRemindNotEnabled => "Please activate reminders first";
		public string CalendarOptionRemindFrequencyLabel(int option) => $"{(option / 60 >= 1 ? $"{option / 60} hour(s)" : $"{option} minute(s)")} before";
		public string CalendarOptionRemindFrequencyChoose => "Select the match reminders you want";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*              CALENDAR OPTIONS REMIND FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarOptionRemindFrequencyUpdated(int[] options) => $"The reminders will now be as follows :\n" +
			$"• {string.Join("\n• ", options.Select(o => o / 60 >= 1 ? $"{o / 60} hour(s) before" : $"{o} minute(s) before"))}";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdAdded(string id) => $"Calendar **{id}** has been linked to this server";
		public string CalendarIdUpdated(string id) => $"Calendar will now be changed to **{id}**";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      ROLE MANAGER ADD COMMAND                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RoleManagerAddTooManyRoles(int nb) => $"You have already more than **{nb}** registered roles as **Manager**..." +
			$"\nIn my experience, you don't need that much, I advise you to create a common role for all";
		public string RoleManagerAddRoleAlreadyExists => "This role is already registered as **Manager**";
		public string RoleManagerAddRoleAdded => "Members with this role will now be able to use commands that require **Manager** permission";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    ROLE MANAGER DELETE COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RoleManagerDeleteNoRoles => "You have no registered role as **Manager**";
		public string RoleManagerDeleteSelectRole => "Select the role you want to remove the **Manager** permission";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    ROLE MANAGER DELETE SELECT                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RoleManagerDeleteSelectRoleDeleted => "Members with this role will no longer be able to use commands requiring **Manager** permission";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      ROLE PLAYER ADD COMMAND                      *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RolePlayerAddTooManyRoles(int nb) => $"You have already more than **{nb}** registered roles as **Player**..." +
			$"\nIn my experience, you don't need that much, I advise you to create a common role for all";
		public string RolePlayerAddRoleAlreadyExists => "This role is already registered as **Player**";
		public string RolePlayerAddRoleAdded => "Members with this role will now be able to use commands that require **Player** permission" +
			"\nIn addition, members will now be displayed in the lists when adding matches to the calendar";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                     ROLE PLAYER DELETE COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RolePlayerDeleteNoRoles => "You have no registered role as **Player**";
		public string RolePlayerDeleteSelectRole => "Select the role you want to remove the **Player** permission";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                     ROLE PLAYER DELETE SELECT                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string RolePlayerDeleteSelectRoleDeleted => "Members with this role will no longer be able to use commands requiring **Player** permission" +
			"\nIn addition, members will no longer be displayed in the lists when adding matches to the calendar";
	}
}
