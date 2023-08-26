namespace Wp.Language.French
{
    public class GlobalFrench : IGlobal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    GLOBAL INFORMATIONS COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EmbedInformations => "Informations";
        public string EmbedDescription => "Bot de gestion d'équipe e-Sport";
        public string EmbedFieldAuthor => "Auteur";
        public string EmbedFieldLastConnection => "Temps de fonctionnement";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      PLAYER ACCOUNTS COMMAND                      *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string PlayerAccountsNoAccounts => "Vous n'avez aucun compte enregistré";

        public string PlayerAccountsEmbedTitle => "Comptes enregistrés";

        public string PlayerAccountsEmbedDescription => "Voici vos différents comptes enregistrés";

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
