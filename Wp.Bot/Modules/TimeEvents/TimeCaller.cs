using Discord;
using Wp.Common.Models;

namespace Wp.Bot.Modules.TimeEvents
{
    public static class TimeCaller
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static void Execute(TimeAction action, IGuild guild)
        {
            switch (action)
            {
                case TimeAction.SCAN_CLASH_OF_CLANS_API:
                    break;
                case TimeAction.DETECT_END_WAR:
                    break;
                case TimeAction.DISPLAY_CALENDAR:
                    Calendar.Display.Execute(guild);
                    break;
                case TimeAction.REMIND_WAR:
                    War.Remind.Execute(guild);
                    break;
                case TimeAction.REMIND_WAR_STATUS:
                    War.Status.Execute(guild);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}
