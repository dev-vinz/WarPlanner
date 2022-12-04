namespace Wp.Language.French
{
	public class AdminFrench : IAdmin
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           GLOBAL CALENDAR                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdNotSet => "Vous devez d'abord enregistrer votre ID de calendrier";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      CALENDAR CHANNEL COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarWillBeDisplayedHere(string user) => $"{user}, c'est ici que je posterai le calendrier\nNe supprimez pas ce message";
		public string CalendarChannelChanged(string channel) => $"Le calendrier apparaîtra désormais dans {channel}";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CalendarIdAdded(string id) => $"Le calendrier **{id}** a été lié à ce serveur";
		public string CalendarIdUpdated(string id) => $"Le calendrier sera désormais remplacé par **{id}**";
	}
}
