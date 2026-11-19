using System.Text.Json;

namespace CodeInvention.CuriosityLab.Common.Extensions
{
    public static class JsonExtension
    {
        public static T Deserialize<T>(this string value) where T : class
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<T>(value, options);
        }

        public static string Serialize<T>(this T value) where T : class
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Serialize(value, options);
        }
    }
}
