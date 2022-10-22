namespace Wp.Database.Models
{
    public enum TimeAction
    {
        /// <summary>
        /// Scans the Clash Of Clans API to detect errors
        /// </summary>
        SCAN_CLASH_OF_CLANS_API = -1,

        /// <summary>
        /// Detects the end of war, to add statistics
        /// </summary>
        DETECT_END_WAR = 0,

        /// <summary>
        /// Displays the calendar in a discord channel
        /// </summary>
        DISPLAY_CALENDAR = 1,

        /// <summary>
        /// Reminds the war to players
        /// </summary>
        REMIND_WAR = 2,

        /// <summary>
        /// Reminds to turn war status off
        /// </summary>
        REMIND_WAR_STATUS = 3,
    }
}
