namespace Wp.Language.English
{
    public class ManagerEnglish : IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN ADD COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanAddAlreadyRegistered(string clan) => $"You have already registered **{clan}** in the guild";
        public string ClanAddRegistered(string clan) => $"Clan **{clan}** added";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NoClanRegisteredToRemove => "No clan was registered in the guild";
        public string ClanCannotRemovedBecauseCurrentlyUsed => "Registered clans are still used in some competitions";
        public string SelectClanToRemove => "Select the clan to delete\nClans still used in competitions cannot be deleted";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanSelectRemoved(string clan) => $"The **{clan}** clan has been removed from the guild";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION ADD COMMAND                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CompetitionNameContainsEmoji => "Competition name must not contain emoji";
        public string ChooseCompetitionMainClan => "Please choose the main clan used in this tournament";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     COMPETITION DELETE COMMAND                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NoCompetitionToDelete => "Sorry, but there's no competition to delete";
        public string ChooseCompetitionToDelete => "Please choose the competition to delete";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionName => "Name";
        public string EditCompetitionResultChannel => "Result channel";
        public string EditCompetitionMainClan => "Main clan";
        public string EditCompetitionSecondClan => "Secondary clan";
        public string EditCompetitionChooseEdition(string competitionName) => $"Choose the modification you want for tournament **{competitionName}**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT MODAL                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionNameModalTitle => "Edit competition's name";
        public string EditCompetitionNameModalField => "Competition's name";
        public string EditCompetitionNameUpdated(string name) => $"Competition's name has been changed to : **{name}**" +
            $"\nThe environment has also been changed";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ADD SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CompetitionNoSecondClanButton => "No secondary clan";
        public string ChooseCompetitionSecondClan => "Please choose the secondary clan used in this tournament\nIt will be taken as a reference when there are clan leagues in the main clan";
        public string CompetitionAdded(string name, string main, string? second) => $"The competition has been added\n" +
            $"\nName : **{name}**" +
            $"\nMain Clan : **{main}**" +
            $"\nSecondary Clan : **{second ?? "None"}**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION DELETE SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CompetitionDeleted(string name) => $"The competition **{name}**, and its environment, have been deleted";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT SELECT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionSelectMainClan => "Choose your new main clan";

        public string EditCompetitionSelectMainClanUpdated(string competition, string name) => $"The main clan used in **{competition}** will now be **{name}**";
        public string EditCompetitionSelectSecondClan => "Choose your new secondary clan";
        public string EditCompetitionSelectSecondClanUpdated(string competition, string name) => $"The secondary clan used in **{competition}** will now be **{name}**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ENVIRONMENT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CouldntCreateCompetitionEnvironment => "An error occurred, I could not create the environment";
        public string CompetitionEnvironmentReferentRoleName(string competition) => $"Referent {competition}";
        public string CompetitionEnvironmentInformationChannel => "informations";
        public string CompetitionEnvironmentFloodChannel => "flood";
        public string CompetitionenvironmentResultChannel => "match-results";
        public string CompetitionEnvironmentVoiceChannel => "Voice";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          WAR ADD COMMAND                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarAddNoCompetition => "You have no competition in progress";
        public string WarAddNoPlayers(uint minTh) => $"You do not own a registered player with a `Player` role, and a TH greater than or equal to {minTh}";
        public string WarAddChooseHour(string opponent, string date) => $"What time is the match scheduled against **{opponent}**, on *{date}* ?";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     WAR ADD COMPETITION SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarAddCompetitionNextPlayers => "Next players";
        public string WarAddCompetitionSelectPlayers(string competition, int nbPages) => $"Please select the players who will play in **{competition}**\n" +
            $"\nPage 1/{nbPages}";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR ADD HOUR SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarAddHourSelectMinutes(string date) => $"Can you be a little more specific than **{date}** ?";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       WAR ADD MINUTES SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarAddMinutesSelectCompetition(string date) => $"Please select the competition you will be playing at **{date}**";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       WAR ADD PLAYERS SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarAddPlayersEnd => "Terminer";
        public string WarAddPlayersSelectPlayers(int currentPage, int nbPages, int nbPlayers) => $"Select the players who will play the match\nCurrently, there are **{nbPlayers}** selected players\n" +
            $"\nPage {currentPage}/{nbPages}";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                    WAR ADD LAST PLAYERS SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarAddLastPlayersMatchAdded(string date, string opponent) => $"Your **{date}** match against **{opponent}** has been added to your calendar" +
            $"\nBonne chance !";
        public string WarAddLastPlayersMatchProblem => "Hmm... there was a problem adding the match. Are you sure I still have the required rights, and that the calendar still exists ?";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR DELETE COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarDeleteNoEvents => "You have no match planned for the next few days";
        public string WarDeleteMatchFromTo(string date, string start, string end) => $"On {date}, from {start} to {end}";
        public string WarDeleteChooseMatch => "Select the match you want to delete";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR DELETE SELECT                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string WarDeleteCannotDelete => "Hmm... there was a problem when the game was deleted. Are you sure that I still have the required rights, and that the calendar still exists ?";
        public string WarDeleteMatchDeleted => "The match has been deleted from your calendar";
    }
}
