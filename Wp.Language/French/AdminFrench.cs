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

        public string CalendarOptionChannelNotSet => $"Veuillez d'abord paramétrer le salon d'affichage en utilisant </calendar channel:1048881562772582440>";
        public string CalendarOptionDisplayEnabled(string channel) => $"L'affichage calendrier a été activé dans {channel}";
        public string CalendarOptionDisplayDisabled => "L'affichage calendrier a été désactivé";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*             CALENDAR OPTIONS DISPLAY FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionDisplayNotEnabled => "Veuillez d'abord activer l'affichage du calendrier";
        public string CalendarOptionDisplayFrequencyPerDayLabel(int option) => $"{option} fois par jour";
        public string CalendarOptionDisplayFrequencyPerDayDescription(int option) => $"Toutes les {Math.Round(24.0 / option, 2)} heures";
        public string CalendarOptionDisplayFrequencyChoose => "Choisissez la fréquence d'affichage du calendrier";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*             CALENDAR OPTIONS DISPLAY FREQUENCY SELECT             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionDisplayFrequencyUpdated(string nbPerDay) => $"La fréquence d'affichage a été définie sur **{nbPerDay} fois par jour**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                   CALENDAR OPTIONS REMIND BUTTON                  *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionRemindEnabled => "Le rappel des matchs aux joueurs a été activé. Par défaut, le rappel est fixé sur `2 heures avant`";
        public string CalendarOptionRemindDisabled => "Le rappel des matchs aux joueurs a été désactivé";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*              CALENDAR OPTIONS REMIND FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionRemindNotEnabled => "Veuillez d'abord activer les rappels";
        public string CalendarOptionRemindFrequencyLabel(int option) => $"{(option / 60 >= 1 ? $"{option / 60} heure(s)" : $"{option} minute(s)")} avant";
        public string CalendarOptionRemindFrequencyChoose => "Sélectionnez les rappels de match que vous souhaitez";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*              CALENDAR OPTIONS REMIND FREQUENCY BUTTON             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarOptionRemindFrequencyUpdated(int[] options) => $"Les rappels seront désormais les suivants :\n" +
            $"• {string.Join("\n• ", options.Select(o => o / 60 >= 1 ? $"{o / 60} heure(s) avant" : $"{o} minute(s) avant"))}";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        CALENDAR SET COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CalendarIdAdded(string id) => $"Le calendrier **{id}** a été lié à ce serveur";
        public string CalendarIdUpdated(string id) => $"Le calendrier sera désormais remplacé par **{id}**";
    }
}
