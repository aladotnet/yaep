namespace System.Text.Json;

/// <summary>
/// Provides extension methods for JSON serialization and deserialization using <see cref="JsonSerializer"/>.
/// </summary>
/// <remarks>
/// <para>
/// This class provides convenient extension methods for converting objects to and from JSON format
/// using System.Text.Json. Both methods support optional configuration of <see cref="JsonSerializerOptions"/>.
/// </para>
/// <para>
/// These methods are thin wrappers around <see cref="JsonSerializer.Serialize{TValue}(TValue, JsonSerializerOptions)"/>
/// and <see cref="JsonSerializer.Deserialize{TValue}(string, JsonSerializerOptions)"/>.
/// </para>
/// </remarks>
public static class JsonSerializationExtensions
{
    /// <summary>
    /// Deserializes a JSON string to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="config">An optional action to configure <see cref="JsonSerializerOptions"/>.</param>
    /// <returns>
    /// The deserialized object of type <typeparamref name="T"/>, or <c>default</c> if the JSON string is null or empty.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If <paramref name="json"/> is null, empty, or whitespace, this method returns <c>default(T)</c>
    /// without attempting deserialization.
    /// </para>
    /// <para>
    /// When <paramref name="config"/> is provided, a new <see cref="JsonSerializerOptions"/> instance
    /// is created and passed to the configuration action.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple deserialization
    /// var user = jsonString.JsonDeserialize&lt;User&gt;();
    ///
    /// // With options configuration
    /// var user = jsonString.JsonDeserialize&lt;User&gt;(options =>
    /// {
    ///     options.PropertyNameCaseInsensitive = true;
    ///     options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    /// });
    /// </code>
    /// </example>
    public static T? JsonDeserialize<T>(this string json, Action<JsonSerializerOptions>? config = null)
    {
        if (json.IsNullOrEmpty())
            return default;

        if (config.IsNotNull())
        {
            var options = new JsonSerializerOptions();
            config!(options);

            return
                 JsonSerializer.Deserialize<T>(json, options);
        }

        return
            JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="this">The object to serialize.</param>
    /// <param name="config">An optional action to configure <see cref="JsonSerializerOptions"/>.</param>
    /// <returns>A JSON string representation of the object.</returns>
    /// <remarks>
    /// <para>
    /// When <paramref name="config"/> is provided, a new <see cref="JsonSerializerOptions"/> instance
    /// is created and passed to the configuration action.
    /// </para>
    /// <para>
    /// If the object is <c>null</c>, this method returns the JSON literal <c>"null"</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple serialization
    /// var json = user.ToJson();
    ///
    /// // With options configuration
    /// var json = user.ToJson(options =>
    /// {
    ///     options.WriteIndented = true;
    ///     options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    /// });
    /// </code>
    /// </example>
    public static string ToJson<T>(this T @this, Action<JsonSerializerOptions>? config = null)
    {
        if (config.IsNotNull())
        {
            var options = new JsonSerializerOptions();
            config!(options);

            return
            JsonSerializer.Serialize(@this, options);
        }

        return
            JsonSerializer.Serialize(@this);
    }
}