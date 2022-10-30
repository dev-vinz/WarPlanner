namespace Wp.Bot.Services.Languages.French
{
    public class FrenchGlobalResponse : IGlobalResponse
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED FIELDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        string IGlobalResponse.ClashOfClansInMaintenance => "Clash Of Clans est actuellement en cours de maintenance, réessayez plus tard";
        string IGlobalResponse.ClashOfClansInvalidIp => "Un problème est survenu, l'erreur a été remontée au développeur. Merci de bien vouloir attendre la correction du problème";
        string IGlobalResponse.ClashOfClansNotFound => "Le tag que vous avez essayé de rechercher n'existe pas sur Clash Of Clans";


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SupportServer => "Serveur support";
    }
}
