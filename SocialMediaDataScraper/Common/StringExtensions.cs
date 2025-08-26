#nullable disable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocialMediaDataScraper.Models
{
    public static class StringExtensions
    {
        public static Dictionary<TKey, TValue> JsonStringToDictionary<TKey, TValue>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<TKey, TValue>();

            json = json.Trim();

            if (!IsValidJson(json))
                return new Dictionary<TKey, TValue>();

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
            }
            catch (Exception)
            {
                return new Dictionary<TKey, TValue>();
            }
        }

        private static bool IsValidJson(string json)
        {
            try
            {
                var token = JToken.Parse(json);
                return token is JObject;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}
