using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods for collection operations including null/empty checks,
/// conditional additions, replacements, removals, and other collection utilities.
/// </summary>
/// <remarks>
/// <para>
/// All methods in this class are optimized for minimal allocations where possible.
/// Methods check for <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>
/// to use Count property instead of enumeration, and use index-based iteration for <see cref="IList{T}"/>.
/// </para>
/// <para>
/// The <see cref="NotNullWhenAttribute"/> is used on null-checking methods to help
/// the compiler understand nullability after the check.
/// </para>
/// </remarks>
public static class CollectionsUtils
{
    /// <summary>
    /// Determines whether the specified array is null or has no elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="arr">The array to check.</param>
    /// <returns>
    /// <c>true</c> if the array is null or contains no elements; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method uses direct Length property access for optimal performance (no allocation).
    /// The <see cref="NotNullWhenAttribute"/> ensures the compiler knows the array is not null when returning <c>false</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// int[]? numbers = GetNumbers();
    /// if (!numbers.IsNullOrEmpty())
    /// {
    ///     // numbers is guaranteed non-null here
    ///     Console.WriteLine($"First element: {numbers[0]}");
    /// }
    /// </code>
    /// </example>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? arr) => arr is null || arr.Length == 0;

    /// <summary>
    /// Determines whether the specified array is not null and contains at least one element.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="arr">The array to check.</param>
    /// <returns>
    /// <c>true</c> if the array is not null and contains at least one element; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This is the inverse of <see cref="IsNullOrEmpty{T}(T[])"/> and is provided for readability.
    /// The <see cref="NotNullWhenAttribute"/> ensures the compiler knows the array is not null when returning <c>true</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// int[]? numbers = GetNumbers();
    /// if (numbers.IsNotEmpty())
    /// {
    ///     // numbers is guaranteed non-null here
    ///     ProcessNumbers(numbers);
    /// }
    /// </code>
    /// </example>
    public static bool IsNotEmpty<T>([NotNullWhen(true)] this T[]? arr) => arr is not null && arr.Length > 0;


    /// <summary>
    /// Determines whether the specified enumerable is null or has no elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="list">The enumerable to check.</param>
    /// <returns>
    /// <c>true</c> if the enumerable is null or contains no elements; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is optimized to avoid enumerator allocation when possible:
    /// </para>
    /// <list type="bullet">
    /// <item><description>For <see cref="ICollection{T}"/>, uses the Count property directly.</description></item>
    /// <item><description>For <see cref="IReadOnlyCollection{T}"/>, uses the Count property directly.</description></item>
    /// <item><description>For other enumerables, falls back to <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// IEnumerable&lt;string&gt;? names = GetNames();
    /// if (!names.IsNullOrEmpty())
    /// {
    ///     // names is guaranteed non-null here
    ///     foreach (var name in names)
    ///     {
    ///         Console.WriteLine(name);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? list)
    {
        if (list is null) return true;
        // Optimize for common collection types to avoid enumerator allocation
        if (list is ICollection<T> collection) return collection.Count == 0;
        if (list is IReadOnlyCollection<T> readOnlyCollection) return readOnlyCollection.Count == 0;
        return !list.Any();
    }

    /// <summary>
    /// Determines whether the specified enumerable is not null and contains at least one element.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="list">The enumerable to check.</param>
    /// <returns>
    /// <c>true</c> if the enumerable is not null and contains at least one element; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This is the inverse of <see cref="IsNullOrEmpty{T}(IEnumerable{T})"/> and uses the same optimizations.
    /// </remarks>
    /// <example>
    /// <code>
    /// IEnumerable&lt;int&gt;? items = GetItems();
    /// if (items.IsNotEmpty())
    /// {
    ///     // items is guaranteed non-null here
    ///     ProcessItems(items);
    /// }
    /// </code>
    /// </example>
    public static bool IsNotEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? list)
    {
        if (list is null) return false;
        // Optimize for common collection types to avoid enumerator allocation
        if (list is ICollection<T> collection) return collection.Count > 0;
        if (list is IReadOnlyCollection<T> readOnlyCollection) return readOnlyCollection.Count > 0;
        return list.Any();
    }

    /// <summary>
    /// Converts the specified enumerable to a read-only list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="items">The enumerable to convert.</param>
    /// <returns>
    /// A <see cref="IReadOnlyList{T}"/> containing all elements from the enumerable,
    /// or an empty read-only list if the enumerable is null.
    /// </returns>
    /// <remarks>
    /// This method creates a new <see cref="List{T}"/> from the enumerable and wraps it
    /// with <see cref="List{T}.AsReadOnly"/>. The returned list is a snapshot and will
    /// not reflect changes to the original enumerable.
    /// </remarks>
    /// <example>
    /// <code>
    /// IEnumerable&lt;string&gt; names = GetNames();
    /// IReadOnlyList&lt;string&gt; readOnlyNames = names.ToReadOnlyList();
    /// // readOnlyNames cannot be modified
    /// </code>
    /// </example>
    public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
        => items?.ToList()?.AsReadOnly() ?? Enumerable.Empty<T>().ToList().AsReadOnly();

    /// <summary>
    /// Returns an empty enumerable if the specified enumerable is null; otherwise, returns the original enumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="list">The enumerable to check.</param>
    /// <returns>
    /// <see cref="Enumerable.Empty{TResult}"/> if the enumerable is null; otherwise, the original enumerable.
    /// </returns>
    /// <remarks>
    /// This method is useful for avoiding null checks in foreach loops and LINQ queries.
    /// Uses the cached <see cref="Enumerable.Empty{TResult}"/> instance, so no allocation occurs for null input.
    /// </remarks>
    /// <example>
    /// <code>
    /// IEnumerable&lt;int&gt;? numbers = GetNumbers();
    /// // Safe to iterate without null check
    /// foreach (var num in numbers.EmptyIfNull())
    /// {
    ///     Console.WriteLine(num);
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
    {
        return list ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Attempts to add an item to the collection if it is not already present.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to add to.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>
    /// <c>true</c> if the item was added; <c>false</c> if the item already exists in the collection.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <remarks>
    /// Uses the collection's default equality comparer via <see cref="ICollection{T}.Contains"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var list = new List&lt;string&gt; { "apple", "banana" };
    /// bool added1 = list.TryAdd("cherry");  // true, list now has 3 items
    /// bool added2 = list.TryAdd("apple");   // false, "apple" already exists
    /// </code>
    /// </example>
    public static bool TryAdd<T>(this ICollection<T> list, T value)
    {
        list.GuardAgainstNull(nameof(list));
        if (list.Contains(value))
            return false;

        list.Add(value);
        return true;
    }

    /// <summary>
    /// Attempts to add an item to the collection if it is not already present, using a custom equality comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to add to.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="comparer">The equality comparer to use for checking duplicates.</param>
    /// <returns>
    /// <c>true</c> if the item was added; <c>false</c> if the item already exists in the collection.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="comparer"/> is null.</exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;string&gt; { "Apple", "Banana" };
    /// bool added = list.TryAdd("apple", StringComparer.OrdinalIgnoreCase); // false, case-insensitive match
    /// </code>
    /// </example>
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
    /// Attempts to add an item to the collection if it is not already present, using a custom comparison function.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to add to.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="comparer">A function that compares two items for equality.</param>
    /// <returns>
    /// <c>true</c> if the item was added; <c>false</c> if the item already exists in the collection.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="comparer"/> is null.</exception>
    /// <remarks>
    /// Uses manual iteration to avoid LINQ closure allocation.
    /// </remarks>
    /// <example>
    /// <code>
    /// var list = new List&lt;Person&gt; { new Person { Id = 1, Name = "John" } };
    /// var newPerson = new Person { Id = 1, Name = "Johnny" };
    /// bool added = list.TryAdd(newPerson, (a, b) => a.Id == b.Id); // false, same Id exists
    /// </code>
    /// </example>
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

    /// <summary>
    /// Adds an item to the collection only if the predicate returns <c>true</c>.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to add to.</param>
    /// <param name="value">The value to potentially add.</param>
    /// <param name="predicate">A function that determines whether the value should be added.</param>
    /// <returns>
    /// <c>true</c> if the item was added; <c>false</c> if the predicate returned <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="predicate"/> is null.</exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt;();
    /// list.AddIf(5, x => x > 0);   // true, 5 is added
    /// list.AddIf(-3, x => x > 0);  // false, -3 is not added
    /// </code>
    /// </example>
    public static bool AddIf<TValue>(this ICollection<TValue> list, TValue value, Func<TValue, bool> predicate)
    {
        list.GuardAgainstNull(nameof(list));
        predicate.GuardAgainstNull(nameof(predicate));

        if (!predicate(value))
            return false;

        list.Add(value);
        return true;
    }

    /// <summary>
    /// Adds an item to the collection only if the predicate returns <c>false</c>.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to add to.</param>
    /// <param name="value">The value to potentially add.</param>
    /// <param name="predicate">A function that determines whether the value should be excluded.</param>
    /// <returns>
    /// <c>true</c> if the item was added; <c>false</c> if the predicate returned <c>true</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// This is the inverse of <see cref="AddIf{TValue}"/>. The item is added when the predicate returns <c>false</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var list = new List&lt;string&gt;();
    /// list.AddIfNot("valid", s => string.IsNullOrEmpty(s));   // true, added (not null/empty)
    /// list.AddIfNot("", s => string.IsNullOrEmpty(s));        // false, not added (is empty)
    /// </code>
    /// </example>
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
    /// Applies a where clause to the enumerable only if the predicate returns <c>true</c>.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the enumerable.</typeparam>
    /// <param name="values">The enumerable to filter.</param>
    /// <param name="whereClause">The filter condition to apply.</param>
    /// <param name="predicate">A function that determines whether to apply the filter.</param>
    /// <returns>
    /// A filtered enumerable if the predicate returns <c>true</c>; otherwise, the original enumerable unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="whereClause"/> or <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// This method is useful for building conditional queries where filters may or may not be applied
    /// based on runtime conditions.
    /// </remarks>
    /// <example>
    /// <code>
    /// var products = GetProducts();
    /// bool filterByCategory = true;
    /// string category = "Electronics";
    ///
    /// var result = products.WhereIf(
    ///     p => p.Category == category,
    ///     () => filterByCategory);
    /// </code>
    /// </example>
    public static IEnumerable<TValue> WhereIf<TValue>(this IEnumerable<TValue> values, Func<TValue, bool> whereClause, Func<bool> predicate)
    {
        predicate.GuardAgainstNull(nameof(predicate));
        whereClause.GuardAgainstNull(nameof(whereClause));

        if (values.IsNullOrEmpty() || !predicate())
            return values;

        return values.Where(whereClause);
    }

    /// <summary>
    /// Replaces all elements in the list that satisfy a condition with a new value.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the list.</typeparam>
    /// <param name="list">The list to modify.</param>
    /// <param name="value">The replacement value.</param>
    /// <param name="predicate">A function that determines which elements to replace.</param>
    /// <returns>
    /// <c>true</c> if at least one element was replaced; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// Uses index-based iteration for in-place replacement without allocation.
    /// All matching elements are replaced, not just the first one.
    /// </remarks>
    /// <example>
    /// <code>
    /// var numbers = new List&lt;int&gt; { 1, 2, 3, 2, 4 };
    /// numbers.ReplaceWhere(0, x => x == 2); // List becomes { 1, 0, 3, 0, 4 }
    /// </code>
    /// </example>
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
    /// Replaces the single element in the list that satisfies a condition with a new value.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the list.</typeparam>
    /// <param name="list">The list to modify.</param>
    /// <param name="value">The replacement value.</param>
    /// <param name="predicate">A function that determines which element to replace.</param>
    /// <returns>
    /// <c>true</c> if exactly one element was found and replaced; <c>false</c> if no matching element was found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when more than one element satisfies the condition.</exception>
    /// <remarks>
    /// <para>
    /// This method enforces single-match semantics similar to LINQ's <see cref="Enumerable.Single{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>.
    /// </para>
    /// <para>
    /// Uses manual iteration to avoid LINQ closure allocation.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var users = new List&lt;User&gt; { new User { Id = 1 }, new User { Id = 2 } };
    /// var updated = new User { Id = 1, Name = "Updated" };
    /// users.ReplaceSingle(updated, u => u.Id == 1); // Replaces user with Id 1
    /// </code>
    /// </example>
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

    /// <summary>
    /// Removes all elements from the collection that satisfy a condition.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to modify.</param>
    /// <param name="predicate">A function that determines which elements to remove.</param>
    /// <returns>
    /// <c>true</c> if at least one element was removed; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// This method is optimized for <see cref="IList{T}"/> implementations by iterating backwards
    /// to allow safe removal during iteration without allocation.
    /// </para>
    /// <para>
    /// For non-list collections, items to remove are collected first (requires allocation),
    /// then removed in a second pass.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var numbers = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// numbers.RemoveWhere(x => x % 2 == 0); // Removes 2 and 4, list becomes { 1, 3, 5 }
    /// </code>
    /// </example>
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
    /// Removes all elements from the collection that do not satisfy a condition.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to modify.</param>
    /// <param name="predicate">A function that determines which elements to keep.</param>
    /// <returns>
    /// <c>true</c> if at least one element was removed; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// This is the inverse of <see cref="RemoveWhere{TValue}"/>. Elements are removed when the predicate returns <c>false</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var numbers = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// numbers.RemoveWhereNot(x => x % 2 == 0); // Keeps only even numbers, list becomes { 2, 4 }
    /// </code>
    /// </example>
    public static bool RemoveWhereNot<TValue>(this ICollection<TValue> list, Func<TValue, bool> predicate)
    {
        return list.RemoveWhere(v => !predicate(v));
    }

    /// <summary>
    /// Executes an action on each element in the enumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="items">The enumerable to iterate over.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <remarks>
    /// This is a convenience method that provides a fluent alternative to a foreach loop.
    /// Note that this method will enumerate the entire collection, so use with caution on infinite sequences.
    /// </remarks>
    /// <example>
    /// <code>
    /// var names = new[] { "Alice", "Bob", "Charlie" };
    /// names.ForEach(name => Console.WriteLine($"Hello, {name}!"));
    /// </code>
    /// </example>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items) action(item);
    }
}
