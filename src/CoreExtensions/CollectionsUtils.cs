﻿using System.Linq;

namespace System.Collections.Generic
{
    public static class CollectionsUtils
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list) => list.IsNull() || !list.Any();

        public static bool IsNotEmpty<T>(this IEnumerable<T> list) => !list.IsNull() && list.Any();

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
            => items?.ToList()?.AsReadOnly()
             ?? Enumerable.Empty<T>()
                .ToList().AsReadOnly();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Adds an item to the collection if the collection does not
        /// contain it. returns ture if added otherwise it returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAdd<T>(this IList<T> list, T value)
        {
            list.GuardAgainstNull(nameof(list));
            if (list.Contains(value))
                return false;

            list.Add(value);
            return true;
        }


        /// <summary>
        /// Adds an item to the collection if the collection does not
        /// contain it using the given comparer to check the equality. returns ture if added otherwise it returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public static bool TryAdd<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
        {
            list.GuardAgainstNull(nameof(list));
            comparer.GuardAgainstNull(nameof(comparer));
            if (list.Contains(value, comparer))
                return false;

            list.Add(value);
            return true;
        }

        /// <summary>
        /// Adds an item to the collection if the collection does not
        /// contain it using the given compare Func to check the equality. returns ture if added otherwise it returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public static bool TryAdd<T>(this IList<T> list, T value, Func<T, T, bool> comparer)
        {
            comparer.GuardAgainstNull(nameof(comparer));
            if (list.Any(v => comparer(v, value)))
                return false;

            list.Add(value);
            return true;
        }

        /// <summary>Adds the given item only if the predicate returns true</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool AddIf<TValue>(this IList<TValue> list, TValue value, Func<TValue, bool> predicate)
        {
            list.GuardAgainstNull(nameof(list));
            predicate.GuardAgainstNull(nameof(predicate));

            if (!predicate(value))
                return false;

            list.Add(value);
            return true;
        }

        /// <summary>Adds the given item only if the predicate returns false</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool AddIfNot<TValue>(this IList<TValue> list, TValue value, Func<TValue, bool> predicate)
        {
            list.GuardAgainstNull(nameof(list));
            predicate.GuardAgainstNull(nameof(predicate));

            if (predicate(value))
                return false;

            list.Add(value);
            return true;
        }


        /// <summary>Replaces all the entries that satisfies a specified condition with the given value</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool ReplaceWhere<TValue>(this IList<TValue> list, TValue value, Func<TValue, bool> predicate)
        {
            list.GuardAgainstNull(nameof(list));
            predicate.GuardAgainstNull(nameof(predicate));

            var items = list.Where(predicate)
                            .ToArray();

            if (items.IsNullOrEmpty())
                return false;

            foreach (var item in items)
            {
                int index = list.IndexOf(item);
                list[index] = value;
            }

            return true;
        }

        /// <summary>
        /// Replaces the only element of a sequence that satisfies a specified condition or
        /// returns false if no such element exists; this method throws an exception if
        /// more than one element satisfies the condition.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool ReplaceSingle<TValue>(this IList<TValue> list, TValue value, Func<TValue, bool> predicate)
        {
            list.GuardAgainstNull(nameof(list));
            predicate.GuardAgainstNull(nameof(predicate));

            var item = list.SingleOrDefault(predicate);

            if (EqualityComparer<TValue>.Default.Equals(item, default(TValue)))                
                return false;

            var index = list.IndexOf(item);
            list[index] = value;
            return true;
        }



        /// <summary>Removes all elements that satisfies a specified condition or returns null if no such elements exists</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool RemoveWhere<TValue>(this IList<TValue> list, Func<TValue, bool> predicate)
        {
            list.GuardAgainstNull(nameof(list));
            predicate.GuardAgainstNull(nameof(predicate));

            var items = list.Where(predicate)
                            .ToArray();

            if (items.IsNullOrEmpty())
                return false;

            foreach (var item in items)
            {
                list.Remove(item);
            }

            return true;
        }

        public static bool RemoveWhereNot<TValue>(this IList<TValue> list, Func<TValue, bool> predicate)
        {
            return list.RemoveWhere(v => !predicate(v));
        }

    }
}
