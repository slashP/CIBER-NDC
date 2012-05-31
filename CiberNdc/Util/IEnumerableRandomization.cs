using System;
using System.Collections.Generic;

namespace CiberNdc.Util
{
    static class IEnumerableRandomization
    {
        /// <summary>
        /// Randomizes the specified collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>The randomized collection</returns>
        public static IEnumerable<T> Randomize<T>(IEnumerable<T> collection)
        {
            // Put all items into a list.
            var list = new List<T>(collection);
            var randomizer = new Random();
            // And pluck them out randomly.
            for (int i = list.Count; i > 0; i--)
            {
                int r = randomizer.Next(0, i);
                yield return list[r];
                list.RemoveAt(r);
            }
        }
    }
}
