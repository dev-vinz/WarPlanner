using System.Collections.Concurrent;

namespace Wp.Discord.ComponentInteraction
{
    public class ComponentStorage
    {
        private static readonly object _lock = new();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static ComponentStorage instance = null!;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets some datas informations, associated by the message id key
        /// </summary>
        public ConcurrentDictionary<ulong, string[]> MessageDatas { get; }

        /* * * * * * * * * * * * * * * * * *\
        |*            SHORTCUTS            *|
        \* * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private ComponentStorage()
        {
            MessageDatas = new ConcurrentDictionary<ulong, string[]>();
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



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           STATIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets an unique instance of this class
        /// </summary>
        /// <returns>The Singleton</returns>
        public static ComponentStorage GetInstance()
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    instance ??= new ComponentStorage();
                }
            }

            return instance;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                              INDEXERS                             *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                         OPERATORS OVERLOAD                        *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



    }
}
