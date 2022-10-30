using Wp.Api;

namespace Wp.Bot.Services.Languages
{
    public interface IGlobalResponse
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED FIELDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        protected string ClashOfClansInMaintenance { get; }
        protected string ClashOfClansInvalidIp { get; }
        protected string ClashOfClansNotFound { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         CLASH OF CLANS API                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClashOfClansError => ClashOfClansApi.Error switch
        {
            Api.Settings.ClashOfClansError.IN_MAINTENANCE => ClashOfClansInMaintenance,
            Api.Settings.ClashOfClansError.INVALID_IP => ClashOfClansInvalidIp,
            Api.Settings.ClashOfClansError.NOT_FOUND => ClashOfClansNotFound,
            _ => ClashOfClansInvalidIp,
        };


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SupportServer { get; }
    }
}
