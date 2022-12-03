namespace Wp.Language
{
    public interface IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        GLOBAL MANAGER SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectDontContainsOption { get; }
        public string UserNotAllowedToInteract { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN ADD COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanAddAlreadyRegistered(string clan);
        public string ClanAddRegistered(string clan);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE COMMAND                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NoClanRegisteredToRemove { get; }
        public string ClanCannotRemovedBecauseCurrentlyUsed { get; }
        public string SelectClanToRemove { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLAN REMOVE SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanSelectRemoved(string clan);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION ADD COMMAND                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ChooseCompetitionMainClan { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionName { get; }
        public string EditCompetitionResultChannel { get; }
        public string EditCompetitionMainClan { get; }
        public string EditCompetitionSecondClan { get; }
        public string EditCompetitionChooseEdition(string competitionName);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT MODEL                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionNameModalTitle { get; }
        public string EditCompetitionNameModalField { get; }
        public string EditCompetitionNameUpdated(string name);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT SELECT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EditCompetitionSelectMainClan { get; }
        public string EditCompetitionSelectMainClanUpdated(string competition, string name);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ADD SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CompetitionNoSecondClanButton { get; }
        public string ChooseCompetitionSecondClan { get; }
        public string CompetitionAdded(string name, string main, string? second = null);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ENVIRONMENT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CouldntCreateCompetitionEnvironment { get; }
        public string CompetitionEnvironmentReferentRoleName(string competition);
        public string CompetitionEnvironmentInformationChannel { get; }
        public string CompetitionEnvironmentFloodChannel { get; }
        public string CompetitionenvironmentResultChannel { get; }
        public string CompetitionEnvironmentVoiceChannel { get; }
    }
}
