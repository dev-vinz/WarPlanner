namespace Wp.Database.Services.Extensions
{
    public static class IEnumerableExtensions
    {
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
