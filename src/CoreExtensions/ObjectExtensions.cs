using System.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Provides extension methods for <see cref="object"/> operations including null checks and default value handling.
/// </summary>
/// <remarks>
/// <para>
/// This class provides fluent null-checking methods that work with the C# compiler's nullable reference type analysis.
/// The <see cref="NotNullWhenAttribute"/> is used to inform the compiler about the nullability state after each check.
/// </para>
/// <para>
/// These methods are constrained to reference types (<c>where T : class</c>) since value types cannot be null
/// in the traditional sense (use <see cref="Nullable{T}"/> for nullable value types).
/// </para>
/// </remarks>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines whether the specified object is null.
    /// </summary>
    /// <typeparam name="T">The type of the object. Must be a reference type.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <returns>
    /// <c>true</c> if the object is null; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The <see cref="NotNullWhenAttribute"/> ensures the compiler knows that when this method returns <c>false</c>,
    /// the object is guaranteed to be non-null. This enables null-state flow analysis in calling code.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// User? user = GetUser();
    /// if (!user.IsNull())
    /// {
    ///     // user is guaranteed non-null here
    ///     Console.WriteLine(user.Name);
    /// }
    /// </code>
    /// </example>
    public static bool IsNull<T>([NotNullWhen(false)] this T? obj)
        where T : class
    {
        return obj is null;
    }

    /// <summary>
    /// Determines whether the specified object is not null.
    /// </summary>
    /// <typeparam name="T">The type of the object. Must be a reference type.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <returns>
    /// <c>true</c> if the object is not null; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is the inverse of <see cref="IsNull{T}"/>. The <see cref="NotNullWhenAttribute"/>
    /// ensures the compiler knows that when this method returns <c>true</c>, the object is guaranteed to be non-null.
    /// </para>
    /// <para>
    /// This method provides a more readable alternative to <c>!obj.IsNull()</c> or <c>obj is not null</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// User? user = GetUser();
    /// if (user.IsNotNull())
    /// {
    ///     // user is guaranteed non-null here
    ///     ProcessUser(user);
    /// }
    /// </code>
    /// </example>
    public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj)
        where T : class
    {
        return !obj.IsNull();
    }

    /// <summary>
    /// Returns the specified default value if the object is null; otherwise, returns the original object.
    /// </summary>
    /// <typeparam name="T">The type of the object. Must be a reference type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="defaultValue">The default value to return if <paramref name="value"/> is null.</param>
    /// <returns>
    /// The original value if not null; otherwise, <paramref name="defaultValue"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method provides a fluent alternative to the null-coalescing operator (<c>??</c>).
    /// It can be useful in method chaining scenarios where the null-coalescing operator would be awkward.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// User? user = GetUser();
    /// var displayName = user.DefaultIfNull(defaultUser).Name;
    ///
    /// // Equivalent to:
    /// var displayName2 = (user ?? defaultUser).Name;
    /// </code>
    /// </example>
    public static T DefaultIfNull<T>(this T? value, T defaultValue)
        where T : class
    {
        if (value.IsNull())
            return defaultValue;

        return value!;
    }
}
