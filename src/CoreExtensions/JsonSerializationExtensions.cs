using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    public static class JsonSerializationExtensions
    {
        
        public static T JsonDeserialize<T>(this string json, Action<JsonSerializerOptions> config = null)
        {
            JsonSerializerOptions options = null;
            if (config.IsNotNull())
            {
                options = new JsonSerializerOptions();
                config(options);

                return
                     JsonSerializer.Deserialize<T>(json,options);
            }

            return
                JsonSerializer.Deserialize<T>(json);

        }

        public static string ToJson<T>(this T @this, Action<JsonSerializerOptions> config = null)
        {
            JsonSerializerOptions options = null;
            if (config.IsNotNull())
            {
                options = new JsonSerializerOptions();
                config(options);

                return
                JsonSerializer.Serialize(@this,options);
            }

            return
                JsonSerializer.Serialize(@this);
        }
    }
}
