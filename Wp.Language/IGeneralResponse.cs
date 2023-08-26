using Wp.Api;

namespace Wp.Language
{
    public interface IGeneralResponse
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED FIELDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        protected string ClashOfClansInMaintenance { get; }
        protected string ClashOfClansInvalidIp { get; }
        protected string ClashOfClansNotFound { get; }
        protected string ClashOfClansUnknownError { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      PRECONDITION ATTRIBUTES                      *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string RequirePremiumAttribute(string premiumLevel);
        public string RequireUserNotInGuild { get; }
        public string RequireUserAttribute { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           CLASH OF CLANS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TownHallFullName { get; }

        public string TownHallShortcut { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLASH OF CLANS API                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClashOfClansError => ClashOfClansApi.Error switch
        {
            Api.Settings.ClashOfClansError.IN_MAINTENANCE => ClashOfClansInMaintenance,
            Api.Settings.ClashOfClansError.INVALID_IP => ClashOfClansInvalidIp,
            Api.Settings.ClashOfClansError.NOT_FOUND => ClashOfClansNotFound,
            _ => ClashOfClansUnknownError,
        };

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         GOOGLE CALENDAR API                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string GoogleCannotAccessCalendar { get; }
        public string GoogleCannotUpdateEvent { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           SOCKET CHANNELS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string NotThePermissionToWrite { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CancelButton { get; }
        public string Documentation { get; }
        public string LinkInvitation { get; }
        public string SupportServer { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     GLOBAL BUTTONS INTERACTION                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ActionCanceledByButton { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            GLOBAL DAYS                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Monday { get; }
        public string Tuesday { get; }
        public string Wednesday { get; }
        public string Thursday { get; }
        public string Friday { get; }
        public string Saturday { get; }
        public string Sunday { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL SELECT                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectOptionsAreEmpty { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            GLOBAL TIMES                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Second { get; }
        public string Seconds { get; }
        public string Minute { get; }
        public string Minutes { get; }
        public string Hour { get; }
        public string Hours { get; }
        public string Day { get; }
        public string Days { get; }

        public string ShortcutSecond { get; }
        public string ShortcutMinute { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          STORAGE COMPONENT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string FailToGetStorageComponentData { get; }
    }
}
