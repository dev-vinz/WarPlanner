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

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CancelButton { get; }
        public string Documentation { get; }
        public string SupportServer { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     GLOBAL BUTTONS INTERACTION                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ActionCanceledByButton { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          STORAGE COMPONENT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string FailToGetStorageComponentData { get; }
    }
}
