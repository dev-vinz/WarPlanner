namespace Wp.Bot.Services.Languages.French
{
    public class PlayerFrench : IPlayer
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAIM MODAL                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TokenInvalid(string account) => $"Le token donné ne vérifie pas le compte **{account}**" +
            $"\nVous trouverez votre token API dans les paramètres avancés du compte";

        public string AccountAlreadyClaimed(string account) => $"**{account}** est déjà enregistré globalement" +
            $"\nSi ce compte vous appartient, veuillez rejoindre le serveur support et ouvrir un ticket";

        public string AccountClaimed(string account) => $"Le compte **{account}** vous a été lié";
    }
}
