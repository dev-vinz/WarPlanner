namespace Wp.Discord.ComponentInteraction
{
    public class ButtonSerializer
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private readonly ulong userId;
        private readonly ulong messageId;
        private readonly string buttonId;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the user id who can interact with the button component
        /// </summary>
        public ulong UserId { get => userId; }

        /// <summary>
        /// Gets the message id of the button
        /// </summary>
        public ulong MessageId { get => messageId; }

        /// <summary>
        /// Gets the discord button component's id
        /// </summary>
        public string ButtonId { get => buttonId; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Represents a button component, wich can be encoded to determine an unique key value
        /// </summary>
        /// <param name="userId">An user id who interacts with the button</param>
        /// <param name="messageId">An discord message id where the button is</param>
        /// <param name="buttonId">A button component id</param>
        public ButtonSerializer(ulong userId, ulong messageId, string buttonId)
        {
            // Inputs
            {
                this.userId = userId;
                this.messageId = messageId;
                this.buttonId = buttonId;
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          ABSTRACT METHODS                         *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Encodes the button component interaction to a string key value
        /// </summary>
        /// <returns>The string key value encoded</returns>
        public string Encode()
        {
            return $"{userId}_{messageId}_{buttonId}";
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
        /// Decodes a string into a ButtonSerializer
        /// </summary>
        /// <param name="encodedValue">The encoded string value to be decoded</param>
        /// <returns>The select serialized with the different values</returns>
        public static ButtonSerializer? Decode(string encodedValue)
        {
            string[] tab = encodedValue.Split('_');

            if (tab.Length != 3) return null;

            string strUserId = tab[0];
            string strChannelId = tab[1];
            string buttonId = tab[2];

            if (!ulong.TryParse(strUserId, out ulong userId)) return null;

            if (!ulong.TryParse(strChannelId, out ulong channelId)) return null;

            if (string.IsNullOrWhiteSpace(buttonId)) return null;

            return new ButtonSerializer(userId, channelId, buttonId);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



    }
}
