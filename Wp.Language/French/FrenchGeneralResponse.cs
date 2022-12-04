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

        public string GoogleCannotAccessCalendar => "Je n'ai pas accès à ce calendrier\nÊtes-vous sûr.e de m'avoir ajouté correctement ?";


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CancelButton => "Annuler";
        public string Documentation => "Documentation";
        public string SupportServer => "Serveur support";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     GLOBAL BUTTONS INTERACTION                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ActionCanceledByButton => "Très bien, j'annule";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          STORAGE COMPONENT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string FailToGetStorageComponentData => "Une erreur s'est produite, je n'ai pas réussi à récupérer les informations précédentes";
    }
}
