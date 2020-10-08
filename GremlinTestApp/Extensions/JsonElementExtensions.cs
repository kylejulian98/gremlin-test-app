using System;
using System.Text.Json;

namespace GremlinTestApp.Extensions
{
    public static class JsonElementExtensions
    {
        public static T? GetVertexPropertyValue<T>(this JsonElement jsonElement)
        {
            var valueJsonElement = jsonElement.GetPropertyElement();
            
            return ConvertJsonElement<T>(valueJsonElement);
        }

        public static T? GetEdgePropertyValue<T>(this JsonElement jsonElement) => ConvertJsonElement<T>(jsonElement);

        private static T? ConvertJsonElement<T>(JsonElement valueJsonElement)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(valueJsonElement.GetString(), typeof(T));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)Convert.ChangeType(valueJsonElement.GetDateTime(), typeof(T));
            }
            else
            {
                throw new NotImplementedException($"Unable to Get Property Value for Type T ({typeof(T)})");
            }
        }

        private static JsonElement GetPropertyElement(this JsonElement jsonElement)
        {
            foreach (var item in jsonElement.EnumerateArray())
            {
                return item.GetProperty("value");
            }

            return default;
        }
    }
}
