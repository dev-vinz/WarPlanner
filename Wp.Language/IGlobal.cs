namespace Wp.Language
{
    public interface IGlobal
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAIM MODAL                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TokenInvalid(string account);
        public string AccountAlreadyClaimed(string account);
        public string AccountClaimed(string account);
    }
}
