namespace Wp.Language
{
    public interface IManager
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          CLAN ADD COMMAND                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string ClanAlreadyRegistered(string clan);
        public string ClanRegistered(string clan);
    }
}
