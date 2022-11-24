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
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SupportServer { get; }
        public string CancelButton { get; }

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
