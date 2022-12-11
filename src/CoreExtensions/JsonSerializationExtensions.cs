namespace System.Text.Json
{
    /// <summary>
    /// json extensions.
    /// </summary>
    public static class JsonSerializationExtensions
    {
        /// <summary>
        /// deserializes the given Type from json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>returns the deserialized object.</returns>
        public static T JsonDeserialize<T>(this string json, Action<JsonSerializerOptions> config = null)
        {
            JsonSerializerOptions options = null;
            if (config.IsNotNull())
            {
                options = new JsonSerializerOptions();
                config(options);

                return
                     JsonSerializer.Deserialize<T>(json, options);
            }

            return
                JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">The this.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>the json representation.</returns>
        public static string ToJson<T>(this T @this, Action<JsonSerializerOptions> config = null)
        {
            JsonSerializerOptions options = null;
            if (config.IsNotNull())
            {
                options = new JsonSerializerOptions();
                config(options);

                return
                JsonSerializer.Serialize(@this, options);
            }

            return
                JsonSerializer.Serialize(@this);
        }
    }
}