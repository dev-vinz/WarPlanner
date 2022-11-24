namespace Wp.Language.French
{
    public class ManagerFrench : IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        GLOBAL MANAGER SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectDontContainsOption => "Une erreur s'est produite, veuillez réessayer la commande";
        public string UserNotAllowedToSelect => "Vous n'êtes pas autorisé à réagir à cette interaction";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN ADD COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanAddAlreadyRegistered(string clan) => $"Vous avez déjà enregistré **{clan}** dans le serveur";
        public string ClanAddRegistered(string clan) => $"Le clan **{clan}** a été ajouté";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NoClanRegisteredToRemove => "Aucun clan n'a été enregistré dans le serveur";
        public string ClanCannotRemovedBecauseCurrentlyUsed => "Les clans enregistrés sont encore utilisés dans certaines compétitions";
        public string SelectClanToRemove => "Sélectionnez le clan à supprimer\nLes clans encore utilisés dans les compétitions ne peuvent pas être supprimés";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanSelectRemoved(string clan) => $"Le clan **{clan}** a été supprimé du serveur";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION ADD COMMAND                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ChooseCompetitionMainClan => "Veuillez choisir le clan principal utilisé lors de ce tournoi";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ADD SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CompetitionNoSecondClanButton => "Pas de clan secondaire";
        public string ChooseCompetitionSecondClan => "Veuillez choisir le clan secondaire utilisé lors de ce tournoi\nIl sera pris comme référence lorsqu'il y a les ligues de clan dans le clan principal";
        public string CompetitionAdded(string name, string main, string? second) => $"La compétition a été ajoutée\n" +
            $"\nNom : **{name}**" +
            $"\nClan Principal : **{main}**" +
            $"\nClan Secondaire : **{second ?? "Aucun"}**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ENVIRONMENT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CouldntCreateCompetitionEnvironment => "Une erreur s'est produite, je n'ai pas pu créer l'environnement";
        public string CompetitionEnvironmentReferentRoleName(string competition) => $"Référent {competition}";
        public string CompetitionEnvironmentInformationChannel => "informations";
        public string CompetitionEnvironmentFloodChannel => "flood";
        public string CompetitionenvironmentResultChannel => "résultats-match";
        public string CompetitionEnvironmentVoiceChannel => "Vocal";
    }
}
