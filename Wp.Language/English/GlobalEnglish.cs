namespace Wp.Language.English
{
    public class GlobalEnglish : IGlobal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      PLAYER CLAIM INTERACTION                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TokenInvalid(string account) => $"The given token does not check the account **{account}**" +
            $"\nYou can find your API token in the advanced account settings";

        public string AccountAlreadyClaimed(string account) => $"**{account}** is already registered globally" +
            $"\nIf this account belongs to you, please join the support server and open a ticket";

        public string AccountClaimed(string account) => $"Account **{account}** has been linked to you";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PLAYER CLAIM MODAL                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string PlayerClaimTitle => "Link your account";
        public string PlayerClaimTagField => "Clash Of Clans account tag";
        public string PlayerClaimTokenField => "Clash Of Clans account token";
    }
}
