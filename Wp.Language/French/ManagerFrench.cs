namespace Wp.Language.French
{
    public class ManagerFrench : IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        GLOBAL MANAGER SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectDontContainsOption => "Une erreur s'est produite, veuillez réessayer la commande";
        public string UserNotAllowed => "Vous n'êtes pas autorisé à réagir à cette interaction";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN ADD COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanAlreadyRegistered(string clan) => $"Vous avez déjà enregistré **{clan}** dans le serveur";
        public string ClanRegistered(string clan) => $"Le clan **{clan}** a été ajouté";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NoClanRegistered => "Aucun clan n'a été enregistré dans le serveur";
        public string ClansCurrentlyUsed => "Les clans enregistrés sont encore utilisés dans certaines compétitions";
        public string SelectClanToRemove => "Sélectionnez le clan à supprimer\nLes clans encore utilisés dans les compétitions ne peuvent pas être supprimés";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanRemoved(string clan) => $"Le clan **{clan}** a été supprimé du serveur";
    }
}
