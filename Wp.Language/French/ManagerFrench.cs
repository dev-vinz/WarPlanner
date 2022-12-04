namespace Wp.Language.French
{
    public class ManagerFrench : IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        GLOBAL MANAGER SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectDontContainsOption => "Une erreur s'est produite, veuillez réessayer la commande";
        public string UserNotAllowedToInteract => "Vous n'êtes pas autorisé à réagir à cette interaction";

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

        public string CompetitionNameContainsEmoji => "Le nom de la compétition ne doit pas contenir d'émoji";
        public string ChooseCompetitionMainClan => "Veuillez choisir le clan principal utilisé lors de ce tournoi";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     COMPETITION DELETE COMMAND                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NoCompetitionToDelete => "Désolé, mais il n'y a aucune compétition à supprimer";
        public string ChooseCompetitionToDelete => "Veuillez choisir la compétition à supprimer";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionName => "Nom";
        public string EditCompetitionResultChannel => "Salon d'annonce des résultats";
        public string EditCompetitionMainClan => "Clan principal";
        public string EditCompetitionSecondClan => "Clan secondaire";
        public string EditCompetitionChooseEdition(string competitionName) => $"Choisissez la modification à faire au tournoi **{competitionName}**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT MODEL                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionNameModalTitle => "Modification du nom de compétition";
        public string EditCompetitionNameModalField => "Nom de la competition";
        public string EditCompetitionNameUpdated(string name) => $"Le nom de la compétition a été changé en : **{name}**" +
            $"\nL'environnement a également été changé";

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
        |*                      COMPETITION DELETE SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CompetitionDeleted(string name) => $"La compétition **{name}**, ainsi que son environnement, a été supprimée";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT SELECT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionSelectMainClan => "Choisissez votre nouveau clan principal";

        public string EditCompetitionSelectMainClanUpdated(string competition, string name) => $"Le clan principal utilisé en **{competition}** sera dorénavant **{name}**";
        public string EditCompetitionSelectSecondClan => "Choisissez votre nouveau clan secondaire";
        public string EditCompetitionSelectSecondClanUpdated(string competition, string name) => $"Le clan secondaire utilisé en **{competition}** sera dorénavant **{name}**";

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
