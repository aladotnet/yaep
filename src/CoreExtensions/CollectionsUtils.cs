using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Collections.Generic;

/// <summary>
/// Collection extensions methods.
/// </summary>
public static class CollectionsUtils
{
    /// <summary>
    /// Checks whether the given arr is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <returns>true if null or empty, false if not.</returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? arr) => arr is null || arr.Length == 0;

    /// <summary>
    /// checks wether the given collection is not null and not empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static bool IsNotEmpty<T>([NotNullWhen(true)] this T[]? arr) => arr is not null && arr.Length > 0;


    /// <summary>
    /// Checks whether the given arr is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns>true if null or empty, false if not.</returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? list)
    {
        if (list is null) return true;
        // Optimize for common collection types to avoid enumerator allocation
        if (list is ICollection<T> collection) return collection.Count == 0;
        if (list is IReadOnlyCollection<T> readOnlyCollection) return readOnlyCollection.Count == 0;
        return !list.Any();
    }

    /// <summary>
    /// checks wether the given collection is not null and not empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsNotEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? list)
    {
        if (list is null) return false;
        // Optimize for common collection types to avoid enumerator allocation
        if (list is ICollection<T> collection) return collection.Count > 0;
        if (list is IReadOnlyCollection<T> readOnlyCollection) return readOnlyCollection.Count > 0;
        return list.Any();
    }

    /// <summary>
    /// Converts the given arr to a readonly arr.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
        => items?.ToList()?.AsReadOnly() ?? Enumerable.Empty<T>().ToList().AsReadOnly();

    /// <summary>
    /// Replaces the given instance with an empty collection if it is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns>Enumerable.Empty if the given collection is null.
    /// </returns>
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
    public static bool TryAdd<T>(this ICollection<T> list, T value)
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
    /// <param name="comparer"></param>
    /// <returns></returns>

    public static bool TryAdd<T>(this ICollection<T> list, T value, IEqualityComparer<T> comparer)
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
    /// <param name="comparer"></param>
    /// <returns></returns>

    public static bool TryAdd<T>(this ICollection<T> list, T value, Func<T, T, bool> comparer)
    {
        list.GuardAgainstNull(nameof(list));
        comparer.GuardAgainstNull(nameof(comparer));

        // Manual iteration to avoid LINQ closure allocation
        foreach (var item in list)
        {
            if (comparer(item, value))
                return false;
        }

        list.Add(value);
        return true;
    }

    /// <summary>Adds the given item only if the predicate returns true</summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="list">The arr.</param>
    /// <param name="value">The value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static bool AddIf<TValue>(this ICollection<TValue> list, TValue value, Func<TValue, bool> predicate)
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
    /// <param name="list">The arr.</param>
    /// <param name="value">The value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static bool AddIfNot<TValue>(this ICollection<TValue> list, TValue value, Func<TValue, bool> predicate)
    {
        list.GuardAgainstNull(nameof(list));
        predicate.GuardAgainstNull(nameof(predicate));

        if (predicate(value))
            return false;

        list.Add(value);
        return true;
    }

    /// <summary>
    /// applies the where clause if the given predicate is true, otherwise returns the given arr.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="values"></param>
    /// <param name="whereClause"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TValue> WhereIf<TValue>(this IEnumerable<TValue> values, Func<TValue, bool> whereClause, Func<bool> predicate)
    {
        predicate.GuardAgainstNull(nameof(predicate));
        whereClause.GuardAgainstNull(nameof(whereClause));

        if (values.IsNullOrEmpty() || !predicate())
            return values;

        return values.Where(whereClause);
    }

    /// <summary>Replaces all the entries that satisfies a specified condition with the given value</summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="list">The arr.</param>
    /// <param name="value">The value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static bool ReplaceWhere<TValue>(this IList<TValue> list, TValue value, Func<TValue, bool> predicate)
    {
        list.GuardAgainstNull(nameof(list));
        predicate.GuardAgainstNull(nameof(predicate));

        // Use index-based iteration to avoid allocation and allow in-place replacement
        var replaced = false;
        for (var i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                list[i] = value;
                replaced = true;
            }
        }

        return replaced;
    }

    /// <summary>
    /// Replaces the only element of a sequence that satisfies a condition or
    /// returns false if no such element exists; this method throws an exception if
    /// more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="list">The arr.</param>
    /// <param name="value">The value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static bool ReplaceSingle<TValue>(this IList<TValue> list, TValue value, Func<TValue, bool> predicate)
    {
        list.GuardAgainstNull(nameof(list));
        predicate.GuardAgainstNull(nameof(predicate));

        // Manual iteration to avoid LINQ closure allocation
        var foundIndex = -1;
        for (var i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                if (foundIndex >= 0)
                    throw new InvalidOperationException("Sequence contains more than one matching element");
                foundIndex = i;
            }
        }

        if (foundIndex < 0)
            return false;

        list[foundIndex] = value;
        return true;
    }

    /// <summary>Removes all elements that satisfies a specified condition or returns null if no such elements exists</summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="list">The arr.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static bool RemoveWhere<TValue>(this ICollection<TValue> list, Func<TValue, bool> predicate)
    {
        list.GuardAgainstNull(nameof(list));
        predicate.GuardAgainstNull(nameof(predicate));

        if (list.IsNullOrEmpty())
            return false;

        // Optimize for IList<T> to avoid allocation by iterating backwards
        if (list is IList<TValue> ilist)
        {
            var removed = false;
            for (var i = ilist.Count - 1; i >= 0; i--)
            {
                if (predicate(ilist[i]))
                {
                    ilist.RemoveAt(i);
                    removed = true;
                }
            }
            return removed;
        }

        // Fallback: collect items to remove (requires allocation)
        // Use stackalloc for small collections to minimize heap allocation
        var toRemove = new List<TValue>();
        foreach (var item in list)
        {
            if (predicate(item))
                toRemove.Add(item);
        }

        if (toRemove.Count == 0)
            return false;

        foreach (var item in toRemove)
        {
            list.Remove(item);
        }

        return true;
    }

    /// <summary>
    /// Remove all entries that matches the condition.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns>true if entries has been removed, false if not.</returns>
    public static bool RemoveWhereNot<TValue>(this ICollection<TValue> list, Func<TValue, bool> predicate)
    {
        return list.RemoveWhere(v => !predicate(v));
    }

    /// <summary>
    /// Iterates over the given collection and executes the action.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> items,Action<T> action)
    {
        foreach (var item in items) action(item);
    }
}
