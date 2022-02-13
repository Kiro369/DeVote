using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DeVote.Extensions
{
    public static class CollectionsExtensions
    {
        /// <summary>
        ///     Adds a value to a ConcurrentDictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dict">The dictionary to operate on.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The element to add.</param>
        /// <exception cref="InvalidOperationException">The key already exists.</exception>
        public static void Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.TryAdd(key, value))
                throw new InvalidOperationException("The operation failed; the key likely exists already.");
        }

        /// <summary>
        ///     Removes a value from a ConcurrentDictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dict">The dictionary to operate on.</param>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="InvalidOperationException">The key doesn't exist.</exception>
        /// <returns>The value that was removed (if any).</returns>
        public static TValue Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            if (!dict.TryRemove(key, out value))
                throw new InvalidOperationException("The operation failed; the key may not exist.");

            return value;
        }

        /// <summary>
        ///     Returns the entry in this list at the given index, or the default value of the element
        ///     type if the index was out of bounds.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list to retrieve from.</param>
        /// <param name="index">The index to try to retrieve at.</param>
        /// <returns>The value, or the default value of the element type.</returns>
        public static T TryGet<T>(this IList<T> list, int index)
        {
            return index >= list.Count ? default(T) : list[index];
        }

        /// <summary>
        ///     Returns the entry in this dictionary at the given key, or the default value of the key
        ///     if none.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dict">The dictionary to operate on.</param>
        /// <param name="key">The key of the element to retrieve.</param>
        /// <returns>The value (if any).</returns>
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            return dict.TryGetValue(key, out val) ? val : default(TValue);
        }

        /// <summary>
        ///     Swaps the position of two elements in a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="self">The list to operate on.</param>
        /// <param name="index1">The first index.</param>
        /// <param name="index2">The second index.</param>
        public static void Swap<T>(this IList<T> self, int index1, int index2)
        {
            T temp = self[index1];
            self[index1] = self[index2];
            self[index2] = temp;
        }

        /// <summary>
        ///     Converts a sequence to a queue.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="self">The sequence to convert.</param>
        /// <returns>A queue built from the given sequence.</returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> self)
        {
            return new Queue<T>(self);
        }

        /// <summary>
        ///     Converts a sequence to a stack.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="self">The sequence to convert.</param>
        /// <returns>A stack built from the given sequence.</returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> self)
        {
            return new Stack<T>(self);
        }

        /// <summary>
        ///     Adds a sequence of elements to a collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="col">The collection to add elements to.</param>
        /// <param name="enumerable">The elements to add.</param>
        public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> enumerable)
        {
            foreach (T cur in enumerable)
                col.Add(cur);
        }

        /// <summary>
        ///     Searchs for characters within a string.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="chars">The characters to find.</param>
        /// <param name="rule">The case rule.</param>
        /// <returns>True if the string contains specified characters matching the rule.</returns>
        public static bool Contains(this string str, string chars, StringComparison rule)
        {
            return str.IndexOf(chars, rule) > -1;
        }

        /// <summary>
        ///     Pads a sequence with items until a desired length is reached.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to pad.</param>
        /// <param name="desiredLength">The desired sequence length.</param>
        /// <param name="generator">The generator function to call when more items are needed.</param>
        /// <returns>A padded sequence.</returns>
        public static IEnumerable<T> Pad<T>(this IEnumerable<T> source, int desiredLength, Func<T> generator)
        {
            return PadIterator(source, desiredLength, generator);
        }

        private static IEnumerable<T> PadIterator<T>(IEnumerable<T> source, int desiredLength, Func<T> generator)
        {
            int count = 0;
            foreach (T item in source)
            {
                count++;
                yield return item;
            }

            int remaining = desiredLength - count;
            for (int i = 0; i < remaining; i++)
                yield return generator();
        }

        /// <summary>
        ///     Execute a given action on a sequence immediately.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The sequence to iterate over.</param>
        /// <param name="act">The action to apply on each element.</param>
        /// <returns>The original sequence.</returns>
        public static IEnumerable<T> With<T>(this IEnumerable<T> source, Action<T> act)
        {
            foreach (T item in source)
                act(item);

            return source; // Return the sequence directly, to avoid deferred execution in an iterator.
        }

        /// <summary>
        ///     Forces execution of a deferred iterator.
        /// </summary>
        /// <typeparam name="T">The type of the items in the sequence.</typeparam>
        /// <param name="source">The sequence to process.</param>
        /// <returns>The original sequence passed to this method.</returns>
        public static IEnumerable<T> Force<T>(this IEnumerable<T> source)
        {

            IEnumerator<T> enumer = source.GetEnumerator();

            while (enumer.MoveNext()) ;

            return source;
        }

        public static int CountWhile<T>(this T[] collection, Predicate<T> match)
        {
            for (int i = 0; i < collection.Length; i++)
                if (!match(collection[i]))
                    return i;
            return collection.Length;
        }
        /// <summary>
        /// Splits an array into 2 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Original Array</param>
        /// <param name="index">Index of separation</param>
        /// <param name="first">First array</param>
        /// <param name="last">Second array</param>
        public static void Split<T>(this T[] source, int index, out T[] first, out T[] last)
        {
            int len2 = source.Length - index;
            first = new T[index];
            last = new T[len2];
            Array.Copy(source, 0, first, 0, index);
            Array.Copy(source, index, last, 0, len2);
        }
        //
        // Summary:
        //     Determines whether this string instance starts with the specified character.
        //
        // Parameters:
        //   value:
        //     The character to compare.
        //
        // Returns:
        //     true if value matches the beginning of this string; otherwise, false.
        public static bool StartsWith<T>(this T[] source, T[] value)
        {
            if (source.Length < value.Length)
                return false;
            for (var i = 0; i < value.Length; i++)
            {
                if (!source[i].Equals(value[i]))
                    return false;
            }
            return true;
        }
    }
}
