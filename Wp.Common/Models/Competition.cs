using Wp.Api;

namespace Wp.Common.Models
{
    public class Competition
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly Guild guild;
        private readonly ulong id;
        private ulong resultId;
        private string name;
        private string mainTag;
        private string? secondTag;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the discord server associated at this instance
        /// </summary>
        public Guild Guild { get => guild; }

        /// <summary>
        /// Gets the discord category's id
        /// </summary>
        public ulong Id { get => id; }

        /// <summary>
        /// Gets / Sets the discord result channel's id
        /// </summary>
        public ulong ResultId { get => resultId; set => resultId = value; }

        /// <summary>
        /// Gets / Sets the name
        /// </summary>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Gets / Sets the Clash Of Clans main clan's tag
        /// </summary>
        public string MainTag { get => mainTag; set => mainTag = value; }

        /// <summary>
        /// Gets / Sets the Clash Of Clans second clan's tag, in case of SCCWL
        /// </summary>
        public string? SecondTag { get => secondTag; set => secondTag = value; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the Clash Of Clans main clan's profile, via the API
        /// </summary>
        public ClashOfClans.Models.Clan MainClan => ClashOfClansApi.Clans.GetByTagAsync(MainTag).Result ?? new ClashOfClans.Models.Clan
        {
            Name = "[DELETED]",
            Tag = mainTag,
        };

        /// <summary>
        /// Gets the Clash Of Clans second clan's profile, via the API
        /// </summary>
        public ClashOfClans.Models.Clan? SecondClan => ClashOfClansApi.Clans.GetByTagAsync(SecondTag!).Result;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents a competition inside a discord server
        /// </summary>
        /// <param name="guild">A discord server, represented by the Guild object</param>
        /// <param name="id">A discord category's id</param>
        /// <param name="resultId">A discord channel's id, where the results are posted</param>
        /// <param name="name">A name</param>
        /// <param name="mainTag">A Clash Of Clans clan's tag</param>
        public Competition(Guild guild, ulong id, ulong resultId, string name, string mainTag)
        {
            // Inputs
            {
                this.guild = guild;
                this.id = id;
                this.resultId = resultId;
                this.name = name;
                this.mainTag = mainTag;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public override bool Equals(object? obj)
        {
            // Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Competition? competition = obj as Competition;

                return Guild == competition?.Guild && Id == competition?.Id;
            }
        }

        public override int GetHashCode()
        {
            return Guild.GetHashCode() ^ Id.GetHashCode();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static bool operator ==(Competition? x, Competition? y)
        {
            if (x is null && y is null)
            {
                return true;
            }
            else
            {
                return x?.Equals(y) ?? false;
            }
        }

        public static bool operator !=(Competition? x, Competition? y)
        {
            return !(x == y);
        }
    }
}
