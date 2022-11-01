namespace Wp.Language.French
{
    public class ManagerFrench : IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN ADD COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanAlreadyRegistered(string clan) => $"Vous avez déjà enregistré **{clan}** dans le serveur";
        public string ClanRegistered(string clan) => $"Le clan **{clan}** a été ajouté";
    }
}
