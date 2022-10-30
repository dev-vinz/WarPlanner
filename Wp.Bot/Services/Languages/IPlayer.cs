namespace Wp.Bot.Services.Languages
{
    public interface IPlayer
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CLAIM MODAL                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string TokenInvalid(string account);
        public string AccountAlreadyClaimed(string account);
        public string AccountClaimed(string account);
    }
}
