using System.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Object Extensions.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines whether this instance is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///   <c>true</c> if the specified object is null; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNull<T>([NotNullWhen(false)] this T? obj)
        where T : class
    {
        return obj is null;
    }

    /// <summary>
    /// Determines whether [is not null].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///   <c>true</c> if [is not null] [the specified object]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj)
        where T : class
    {
        return !obj.IsNull();
    }

    /// <summary>
    /// Gets the given defaultValue if the value is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public static T DefaultIfNull<T>(this T? value, T defaultValue)
        where T : class
    {
        if (value.IsNull())
            return defaultValue;

        return value!;
    }
}
