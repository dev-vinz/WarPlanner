namespace Wp.Discord.ComponentInteraction
{
    public class SelectSerializer
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly ulong userId;
        private readonly ulong messageId;
        private readonly string selectId;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the user id who can interact with the select component
        /// </summary>
        public ulong UserId { get => userId; }

        /// <summary>
        /// Gets the message id of the select
        /// </summary>
        public ulong MessageId { get => messageId; }

        /// <summary>
        /// Gets the discord select component's id
        /// </summary>
        public string SelectId { get => selectId; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents a select component, wich can be encoded to determine an unique key value
        /// </summary>
        /// <param name="userId">An user id who interacts with the select</param>
        /// <param name="messageId">An discord message id where the select is</param>
        /// <param name="selectId">A select component id</param>
        public SelectSerializer(ulong userId, ulong messageId, string selectId)
        {
            // Inputs
            {
                this.userId = userId;
                this.messageId = messageId;
                this.selectId = selectId;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Encodes the select component interaction to a string key value
        /// </summary>
        /// <returns>The string key value encoded</returns>
        public string Encode()
        {
            return $"{userId}_{messageId}_{selectId}";
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         PROTECTED METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          OVERRIDE METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public override string ToString()
        {
            return Encode();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Decodes a string into a SelectSerializer
        /// </summary>
        /// <param name="encodedValue">The encoded string value to be decoded</param>
        /// <returns>The select serialized with the different values</returns>
        public static SelectSerializer? Decode(string encodedValue)
        {
            string[] tab = encodedValue.Split('_');

            if (tab.Length != 3) return null;

            string strUserId = tab[0];
            string strChannelId = tab[1];
            string selectId = tab[2];

            if (!ulong.TryParse(strUserId, out ulong userId)) return null;

            if (!ulong.TryParse(strChannelId, out ulong channelId)) return null;

            if (string.IsNullOrWhiteSpace(selectId)) return null;

            return new SelectSerializer(userId, channelId, selectId);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



    }
}
