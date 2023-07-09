namespace Wp.Bot.Modules
{
    public static class IdProvider
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL ACTIONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string GLOBAL_CANCEL_BUTTON = "global_cancel_button";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           ADMIN COMMANDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public const string ADMIN_LANGUAGE_SELECT = "admin_language_select";

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
        public const string COMPETITION_ADD_BUTTON_CREATE_ENVIRONMENT = "competition_add_button_create_environment";
        public const string COMPETITION_ADD_BUTTON_CREATE_NO_ENVIRONMENT = "competition_add_button_create_no_environment";
        public const string COMPETITION_ADD_BUTTON_NO_SECOND_CLAN = "competition_add_button_no_second_clan";

        public const string COMPETITION_DELETE_SELECT = "competition_delete_select";

        public const string COMPETITION_EDIT_BUTTON_NAME = "competition_edit_button_name";
        public const string COMPETITION_EDIT_BUTTON_MAIN_CLAN = "competition_edit_button_main_clan";
        public const string COMPETITION_EDIT_BUTTON_RESULT_CHANNEL = "competition_edit_button_result_channel";
        public const string COMPETITION_EDIT_BUTTON_NEXT_RESULT_CHANNEL = "competition_edit_button_next_result_channel";
        public const string COMPETITION_EDIT_BUTTON_SECOND_CLAN = "competition_edit_button_second_clan";
        public const string COMPETITION_EDIT_SELECT_MAIN_CLAN = "competition_edit_select_main_clan";
        public const string COMPETITION_EDIT_SELECT_RESULT_CHANNEL = "competition_edit_select_result_channel";
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

        public const string WAR_EDIT_SELECT_ADD_PLAYERS = "war_edit_select_add_players";
        public const string WAR_EDIT_SELECT_ADD_LAST_PLAYERS = "war_edit_select_add_last_players";
        public const string WAR_EDIT_SELECT_CHOOSE_UPDATE = "war_edit_select_choose_update";
        public const string WAR_EDIT_SELECT_DAY = "war_edit_select_day";
        public const string WAR_EDIT_SELECT_FORMAT_PREPARATION = "war_edit_select_format_preparation";
        public const string WAR_EDIT_SELECT_FORMAT_WAR = "war_edit_select_format_war";
        public const string WAR_EDIT_SELECT_REMOVE_PLAYERS = "war_edit_select_remove_players";
        public const string WAR_EDIT_SELECT_REMOVE_LAST_PLAYERS = "war_edit_select_remove_last_players";
        public const string WAR_EDIT_SELECT_START_HOUR = "war_edit_select_start_hour";
        public const string WAR_EDIT_SELECT_START_MINUTE = "war_edit_select_start_minute";
        public const string WAR_EDIT_BUTTON_ADD_PLAYER = "war_edit_button_add_player";
        public const string WAR_EDIT_BUTTON_ADD_LAST_NEXT_PLAYERS = "war_edit_button_add_last_next_players";
        public const string WAR_EDIT_BUTTON_ADD_NEXT_PLAYERS = "war_edit_button_add_next_players";
        public const string WAR_EDIT_BUTTON_DAY = "war_edit_button_day";
        public const string WAR_EDIT_BUTTON_FORMAT = "war_edit_button_format";
        public const string WAR_EDIT_BUTTON_OPPONENT = "war_edit_button_opponent";
        public const string WAR_EDIT_BUTTON_REMOVE_PLAYER = "war_edit_button_remove_player";
        public const string WAR_EDIT_BUTTON_REMOVE_LAST_NEXT_PLAYERS = "war_edit_button_remove_last_next_players";
        public const string WAR_EDIT_BUTTON_REMOVE_NEXT_PLAYERS = "war_edit_button_remove_next_players";
        public const string WAR_EDIT_BUTTON_START_HOUR = "war_edit_button_start_hour";
    }
}
