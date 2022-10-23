namespace Wp.Database.Services.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Performs the specified action on each element on the IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action">The action to perform on each element of the IEnumerable</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Makes a deep copy of an IEnumerable and returns it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <returns>The deep copy of an IEnumerable</returns>
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> enumeration)
        {
            return enumeration
                .CopyAsArray()
                .AsEnumerable();
        }

        /// <summary>
        /// Makes a deep copy of an IEnumerable and returns it as an Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <returns>The deep copy of an IEnumerable</returns>
        public static T[] CopyAsArray<T>(this IEnumerable<T> enumeration)
        {
            int count = enumeration.Count();

            T[] final = new T[count];
            Array.Copy(enumeration.ToArray(), final, count);

            return final;
        }

        /// <summary>
        /// Makes a deep copy of an IEnumerable and returns it as a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <returns>The deep copy of an IEnumerable</returns>
        public static IList<T> CopyAsList<T>(this IEnumerable<T> enumeration)
        {
            return enumeration
                .CopyAsArray()
                .ToList();
        }
    }
}
