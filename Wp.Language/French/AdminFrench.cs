namespace Wp.Language.French
{
	public class AdminFrench : IAdmin
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdAdded(string id) => $"Le calendrier **{id}** a été lié à ce serveur";
		public string CalendarIdUpdated(string id) => $"Le calendrier sera désormais remplacé par **{id}**";
	}
}
