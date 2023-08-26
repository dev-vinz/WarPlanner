namespace Wp.Language
{
    public interface IGlobal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                    GLOBAL INFORMATIONS COMMAND                    *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string EmbedInformations { get; }
        public string EmbedDescription { get; }
        public string EmbedFieldAuthor { get; }
        public string EmbedFieldLastConnection { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                      PLAYER ACCOUNTS COMMAND                      *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string PlayerAccountsNoAccounts { get; }

        public string PlayerAccountsEmbedTitle { get; }

        public string PlayerAccountsEmbedDescription { get; }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                      PLAYER CLAIM INTERACTION                     *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TokenInvalid(string account);
        public string AccountAlreadyClaimed(string account);
        public string AccountClaimed(string account);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PLAYER CLAIM MODAL                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string PlayerClaimTitle { get; }
        public string PlayerClaimTagField { get; }
        public string PlayerClaimTokenField { get; }
    }
}
