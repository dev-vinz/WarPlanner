namespace Wp.Language.French
{
    public class GlobalFrench : IGlobal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    GLOBAL INFORMATIONS COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EmbedInformations => "Informations";
        public string EmbedDescription => "Projet réalisé dans le cadre de la Haute-École ARC, lors des cours P3";
        public string EmbedFieldAuthor => "Auteur";
        public string EmbedFieldYear => "Année de réalisation";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      PLAYER CLAIM INTERACTION                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TokenInvalid(string account) => $"Le token donné ne vérifie pas le compte **{account}**" +
            $"\nVous trouverez votre token API dans les paramètres avancés du compte";

        public string AccountAlreadyClaimed(string account) => $"**{account}** est déjà enregistré globalement" +
            $"\nSi ce compte vous appartient, veuillez rejoindre le serveur support et ouvrir un ticket";

        public string AccountClaimed(string account) => $"Le compte **{account}** vous a été lié";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PLAYER CLAIM MODAL                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string PlayerClaimTitle => "Lie ton compte";
        public string PlayerClaimTagField => "Tag du compte Clash Of Clans";
        public string PlayerClaimTokenField => "Token du compte Clash Of Clans";
    }
}
