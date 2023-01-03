namespace Wp.Language.English
{
    public class EnglishGeneralResponse : IGeneralResponse
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED FIELDS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        string IGeneralResponse.ClashOfClansInMaintenance => "Clash Of Clans is currently in maintenance, try again later";
        string IGeneralResponse.ClashOfClansInvalidIp => "There was a problem with the API. Please try again later";
        string IGeneralResponse.ClashOfClansNotFound => "The tag you tried to search does not exist on Clash Of Clans";
        string IGeneralResponse.ClashOfClansUnknownError => "A problem has occurred, the error has been reported to the developer. Please wait for the problem to be fixed";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         GOOGLE CALENDAR API                       *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string GoogleCannotAccessCalendar => "I do not have access to this calendar\nAre you sure you added me correctly ?";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           SOCKET CHANNELS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ChannelNotText => "The specified salon is not a textual salon";
        public string NotThePermissionToWrite => "I have no permission to speak in this salon";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL BUTTONS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string CancelButton => "Cancel";
        public string Documentation => "Documentation";
        public string LinkInvitation => "Invite link";
        public string SupportServer => "Support guild";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                     GLOBAL BUTTONS INTERACTION                    *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ActionCanceledByButton => "All right, I'll cancel";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            GLOBAL DAYS                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Monday => "Monday";
        public string Tuesday => "Tuesday";
        public string Wednesday => "Wednesday";
        public string Thursday => "Thursday";
        public string Friday => "Friday";
        public string Saturday => "Saturday";
        public string Sunday => "Sunday";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           GLOBAL SELECT                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string SelectOptionsAreEmpty => "You have not selected any item";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            GLOBAL TIMES                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string Second => "second";
        public string Seconds => "seconds";
        public string Minute => "minute";
        public string Minutes => "minutes";
        public string Hour => "hour";
        public string Hours => "hours";
        public string Day => "day";
        public string Days => "days";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          STORAGE COMPONENT                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string FailToGetStorageComponentData => "An error occurred, I failed to retrieve previous informations";
    }
}
