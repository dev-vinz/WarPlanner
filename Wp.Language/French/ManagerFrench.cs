namespace Wp.Language.French
{
	public class ManagerFrench : IManager
	{
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
        |*                       COMPETITION ADD BUTTON                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CompetitionAddWithEnvironment => "Avec environnement";
		public string CompetitionAddWithoutEnvironment => "Sans environnement";
		public string CompetitionAddChooseEnvironment => "Veuillez choisir si vous désirez un environnement intégré, avec des salons textuels et vocaux";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     COMPETITION DELETE COMMAND                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string NoCompetitionToDelete => "Désolé, mais il n'y a aucune compétition à supprimer";
		public string ChooseCompetitionToDelete => "Veuillez choisir la compétition à supprimer";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT BUTTON                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionNextChannels => "Salons suivants";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionName => "Nom";
		public string EditCompetitionResultChannel => "Salon d'annonce des résultats";
		public string EditCompetitionMainClan => "Clan principal";
		public string EditCompetitionSecondClan => "Clan secondaire";
		public string EditCompetitionChooseEdition(string competitionName) => $"Choisissez la modification à faire au tournoi **{competitionName}**";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT MODAL                      *|
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

		public string CompetitionDeleted(string name) => $"La compétition **{name}**, ainsi que son environnement, ont été supprimés";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT SELECT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionSelectMainClan => "Choisissez votre nouveau clan principal";
		public string EditCompetitionSelectMainClanUpdated(string competition, string name) => $"Le clan principal utilisé en **{competition}** sera dorénavant **{name}**";
		public string EditCompetitionSelectNewResultChannel => "Sélectionnez le nouveau salon de résultat";
		public string EditCompetitionSelectNewResultChannelUpdated(string competition, string name) => $"Le nouveau salon où sera posté les résultats des matchs en **{competition}** sera dorénavant **{name}**";
		public string EditCompetitionSelectSecondClan => "Choisissez votre nouveau clan secondaire";
		public string EditCompetitionSelectSecondClanUpdated(string competition, string name) => $"Le clan secondaire utilisé en **{competition}** sera dorénavant **{name}**";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ENVIRONMENT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CouldntCreateCompetitionEnvironment => "Une erreur s'est produite, je n'ai pas pu créer l'environnement";
		public string CompetitionEnvironmentReferentRoleName(string competition) => $"Référent {competition}";
		public string CompetitionEnvironmentInformationChannel => "informations";
		public string CompetitionEnvironmentFloodChannel => "flood";
		public string CompetitionenvironmentResultChannel => "résultats–match";
		public string CompetitionEnvironmentVoiceChannel => "Vocal";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          WAR ADD COMMAND                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddNoCompetition => "Vous n'avez aucune compétition en cours";
		public string WarAddNoPlayers(uint minTh) => $"Vous ne possédez aucun joueur–se enregistré possèdant un rôle `Joueur`, et ayant un HDV supérieur ou égal à {minTh}";
		public string WarAddChooseHour(string opponent, string date) => $"À quelle heure est prévu le match contre **{opponent}**, le *{date}* ?";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     WAR ADD COMPETITION SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddCompetitionNextPlayers => "Joueur–se–s suivant–e–s";
		public string WarAddCompetitionSelectPlayers(string competition, int nbPages) => $"Veuillez sélectionner les joueur–se–s qui joueront en **{competition}**\n" +
			$"\nPage 1/{nbPages}";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR ADD HOUR SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddHourSelectMinutes(string date) => $"Pouvez–vous être à peine plus précis–e que **{date}** ?";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       WAR ADD MINUTES SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddMinutesSelectCompetition(string date) => $"Veuillez sélectionner la compétition dans laquelle vous jouerez à **{date}**";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       WAR ADD PLAYERS SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddPlayersEnd => "Terminer";
		public string WarAddPlayersSelectPlayers(int currentPage, int nbPages, int nbPlayers) => $"Sélectionnez les joueur–se–s qui joueront le match\nActuellement, il y a **{nbPlayers}** joueur–se–s sélectionné–e–s\n" +
			$"\nPage {currentPage}/{nbPages}";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                    WAR ADD LAST PLAYERS SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddLastPlayersMatchAdded(string date, string opponent) => $"Votre match du **{date}** contre **{opponent}** a été ajouté à votre calendrier" +
			$"\nBonne chance !";
		public string WarAddLastPlayersMatchProblem => "Hmm... il y a eu un problème lors de l'ajout du match. Êtes-vous sûr–e que je possède encore les droits requis, et que le calendrier existe toujours ?";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR DELETE COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarDeleteNoEvents => "Vous n'avez aucun match prévu ces prochains jours";
		public string WarDeleteMatchFromTo(string date, string start, string end) => $"Le {date}, de {start} à {end}";
		public string WarDeleteChooseMatch => "Sélectionnez le match que vous voulez supprimer";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR DELETE SELECT                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarDeleteCannotDelete => "Hmm... il y a eu un problème lors de la suppression du match. Êtes-vous sûr–e que je possède encore les droits requis, et que le calendrier existe toujours ?";
		public string WarDeleteMatchDeleted => "Le match a été supprimé de votre calendrier";
	}
}
