namespace Wp.Language
{
    public interface IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        GLOBAL MANAGER SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectDontContainsOption { get; }
        public string UserNotAllowedToSelect { get; }

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
