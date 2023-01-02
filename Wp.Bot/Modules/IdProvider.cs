namespace Wp.Bot.Modules
{
    public static class IdProvider
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL ACTIONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string GLOBAL_CANCEL_BUTTON = "global_cancel_button";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CALENDAR COMMANDS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string CALENDAR_OPTIONS_BUTTON_DISPLAY = "calendar_options_button_display";
        public const string CALENDAR_OPTIONS_BUTTON_DISPLAY_FREQUENCY = "calendar_options_button_display_frequency";
        public const string CALENDAR_OPTIONS_BUTTON_REMIND_WAR = "calendar_options_button_remind_war";
        public const string CALENDAR_OPTIONS_BUTTON_CHOOSE_REMIND_WAR = "calendar_options_button_choose_remind_war";

        public const string CALENDAR_OPTIONS_DISPLAY_SELECT_DISPLAY_FREQUENCY = "calendar_options_display_select_display_frequency";
        public const string CALENDAR_OPTIONS_REMIND_WAR_SELECT_NUMBER_REMINDS = "calendar_options_remind_war_select_number_reminds";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           CLAN COMMANDS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string CLAN_REMOVE_SELECT_MENU = "clan_remove_select_menu";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                        COMPETITION COMMANDS                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string COMPETITION_ADD_SELECT_MAIN_CLAN = "competition_add_select_main_clan";
        public const string COMPETITION_ADD_SELECT_SECOND_CLAN = "competition_add_select_second_clan";
        public const string COMPETITION_ADD_BUTTON_NO_SECOND_CLAN = "competition_add_button_no_second_clan";

        public const string COMPETITION_DELETE_SELECT = "competition_delete_select";

        public const string COMPETITION_EDIT_BUTTON_NAME = "competition_edit_button_name";
        public const string COMPETITION_EDIT_BUTTON_RESULT_CHANNEL = "competition_edit_button_result_channel";
        public const string COMPETITION_EDIT_BUTTON_MAIN_CLAN = "competition_edit_button_main_clan";
        public const string COMPETITION_EDIT_BUTTON_SECOND_CLAN = "competition_edit_button_second_clan";
        public const string COMPETITION_EDIT_SELECT_MAIN_CLAN = "competition_edit_select_main_clan";
        public const string COMPETITION_EDIT_SELECT_SECOND_CLAN = "competition_edit_select_second_clan";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           ROLE COMMANDS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string ROLE_MANAGER_DELETE_SELECT = "role_manager_delete_select";
        public const string ROLE_PLAYER_DELETE_SELECT = "role_player_delete_select";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            WAR COMMANDS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string WAR_ADD_SELECT_HOUR = "war_add_select_hour";
        public const string WAR_ADD_SELECT_MINUTES = "war_add_select_minutes";
        public const string WAR_ADD_SELECT_COMPETITION = "war_add_select_competition";
        public const string WAR_ADD_SELECT_PLAYERS = "war_add_select_players";
        public const string WAR_ADD_SELECT_LAST_PLAYERS = "war_add_select_last_players";
        public const string WAR_ADD_BUTTON_LAST_NEXT_PLAYERS = "war_add_button_last_next_players";
        public const string WAR_ADD_BUTTON_NEXT_PLAYERS = "war_add_button_next_players";

        public const string WAR_DELETE_SELECT_EVENT = "war_delete_select_event";
    }
}
