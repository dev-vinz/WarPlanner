﻿namespace Wp.Language.French
{
    public class FrenchGeneralResponse : IGeneralResponse
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED FIELDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        string IGeneralResponse.ClashOfClansInMaintenance => "Clash Of Clans est actuellement en cours de maintenance, réessayez plus tard";
        string IGeneralResponse.ClashOfClansInvalidIp => "Un problème est survenu avec l'API. Merci de bien vouloir réessayer plus tard";
        string IGeneralResponse.ClashOfClansNotFound => "Le tag que vous avez essayé de rechercher n'existe pas sur Clash Of Clans";
        string IGeneralResponse.ClashOfClansUnknownError => "Un problème est survenu, l'erreur a été remontée au développeur. Merci de bien vouloir attendre la correction du problème";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         GOOGLE CALENDAR API                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string GoogleCannotAccessCalendar => "Je n'ai pas accès à ce calendrier\nÊtes-vous sûr-e de m'avoir ajouté correctement ?";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           SOCKET CHANNELS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ChannelNotText => "Le salon spécifié n'est pas un salon textuel";
        public string NotThePermissionToWrite => "Je n'ai pas la permission de parler dans ce salon";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CancelButton => "Annuler";
        public string Documentation => "Documentation";
        public string LinkInvitation => "Lien d'invitation";
        public string SupportServer => "Serveur support";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     GLOBAL BUTTONS INTERACTION                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ActionCanceledByButton => "Très bien, j'annule";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            GLOBAL DAYS                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Monday => "Lundi";
        public string Tuesday => "Mardi";
        public string Wednesday => "Mercredi";
        public string Thursday => "Jeudi";
        public string Friday => "Vendredi";
        public string Saturday => "Samedi";
        public string Sunday => "Dimanche";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL SELECT                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectOptionsAreEmpty => "Vous n'avez séléctionné aucun élément";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            GLOBAL TIMES                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Second => "seconde";
        public string Seconds => "secondes";
        public string Minute => "minute";
        public string Minutes => "minutes";
        public string Hour => "heure";
        public string Hours => "heures";
        public string Day => "jour";
        public string Days => "jours";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          STORAGE COMPONENT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string FailToGetStorageComponentData => "Une erreur s'est produite, je n'ai pas réussi à récupérer les informations précédentes";
    }
}
