namespace Wp.Language
{
	public interface IManager
	{
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

		public string CompetitionNameContainsEmoji { get; }
		public string ChooseCompetitionMainClan { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ADD BUTTON                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CompetitionAddWithEnvironment { get; }
		public string CompetitionAddWithoutEnvironment { get; }
		public string CompetitionAddChooseEnvironment { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     COMPETITION DELETE COMMAND                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string NoCompetitionToDelete { get; }
		public string ChooseCompetitionToDelete { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT BUTTON                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionNextChannels { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION EDIT COMMAND                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionName { get; }
		public string EditCompetitionResultChannel { get; }
		public string EditCompetitionMainClan { get; }
		public string EditCompetitionSecondClan { get; }
		public string EditCompetitionChooseEdition(string competitionName);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT MODAL                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionNameModalTitle { get; }
		public string EditCompetitionNameModalField { get; }
		public string EditCompetitionNameUpdated(string name);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ADD SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CompetitionNoSecondClanButton { get; }
		public string ChooseCompetitionSecondClan { get; }
		public string CompetitionAdded(string name, string main, string? second = null);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      COMPETITION DELETE SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CompetitionDeleted(string name);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION EDIT SELECT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string EditCompetitionSelectMainClan { get; }
		public string EditCompetitionSelectMainClanUpdated(string competition, string name);
		public string EditCompetitionSelectNewResultChannel { get; }
		public string EditCompetitionSelectNewResultChannelUpdated(string competition, string name);
		public string EditCompetitionSelectSecondClan { get; }
		public string EditCompetitionSelectSecondClanUpdated(string competition, string name);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       COMPETITION ENVIRONMENT                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string CouldntCreateCompetitionEnvironment { get; }
		public string CompetitionEnvironmentReferentRoleName(string competition);
		public string CompetitionEnvironmentInformationChannel { get; }
		public string CompetitionEnvironmentFloodChannel { get; }
		public string CompetitionenvironmentResultChannel { get; }
		public string CompetitionEnvironmentVoiceChannel { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          WAR ADD COMMAND                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddNoCompetition { get; }
		public string WarAddNoPlayers(uint minTh);
		public string WarAddChooseHour(string opponent, string date);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     WAR ADD COMPETITION SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddCompetitionNextPlayers { get; }
		public string WarAddCompetitionSelectPlayers(string competition, int nbPages);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR ADD HOUR SELECT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddHourSelectMinutes(string date);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       WAR ADD MINUTES SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddMinutesSelectCompetition(string date);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                       WAR ADD PLAYERS SELECT                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddPlayersEnd { get; }
		public string WarAddPlayersSelectPlayers(int currentPage, int nbPages, int nbPlayers);

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                    WAR ADD LAST PLAYERS SELECT                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarAddLastPlayersMatchAdded(string date, string opponent);
		public string WarAddLastPlayersMatchProblem { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR DELETE COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarDeleteNoEvents { get; }
		public string WarDeleteMatchFromTo(string date, string start, string end);
		public string WarDeleteChooseMatch { get; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        WAR DELETE SELECT                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string WarDeleteCannotDelete { get; }
		public string WarDeleteMatchDeleted { get; }
	}
}
