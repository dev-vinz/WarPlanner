namespace Wp.Language.French
{
    public class FrenchGeneralResponse : IGeneralResponse
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED FIELDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        string IGeneralResponse.ClashOfClansInMaintenance => "Clash Of Clans est actuellement en cours de maintenance, réessayez plus tard";
        string IGeneralResponse.ClashOfClansInvalidIp => "Un problème est survenu, l'erreur a été remontée au développeur. Merci de bien vouloir attendre la correction du problème";
        string IGeneralResponse.ClashOfClansNotFound => "Le tag que vous avez essayé de rechercher n'existe pas sur Clash Of Clans";


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SupportServer => "Serveur support";
    }
}
