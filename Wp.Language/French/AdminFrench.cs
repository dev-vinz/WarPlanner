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
		|*                      CALENDAR OPTIONS COMMAND                     *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionsDisplayed(bool enabled) => enabled ? "Désactiver l'affichage" : "Activer l'affichage";
        public string CalendarOptionsFrequency(string? frequency) => $"Fréquence d'affichage ({frequency ?? "0"})";
        public string CalendarOptionsRemind(bool enabled) => enabled ? "Désactiver les rappels" : "Activer les rappels";
        public string CalendarOptionsChooseRemind(string? reminds) => $"Nombre de rappels ({reminds ?? "0"})";
        public string CalendarChooseOption => "Cliquez sur un des boutons ci-dessous pour changer les paramètres du calendrier";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                   CALENDAR OPTIONS DISPLAY BUTTON                 *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionChanneNotSet => $"Veuillez d'abord paramétrer le salon d'affichage en utilisant </calendar channel:1048881562772582440>";
        public string CalendarOptionsDisplayEnabled(string channel) => $"L'affichage calendrier a été activé dans {channel}";
        public string CalendarOptionsDisplayDisabled => "L'affichage calendrier a été désactivé";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarIdAdded(string id) => $"Le calendrier **{id}** a été lié à ce serveur";
        public string CalendarIdUpdated(string id) => $"Le calendrier sera désormais remplacé par **{id}**";
    }
}
