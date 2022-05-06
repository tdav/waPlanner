using Newtonsoft.Json;

namespace waPlanner.Utils
{
    public static class ObectsExtensions
    {
        public static string ToJson(this object inParam, Formatting format = Formatting.None, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
        {
            if (inParam == null)
                return "{}";
            return JsonConvert.SerializeObject(inParam, format, new JsonSerializerSettings { NullValueHandling = nullValueHandling });
        }

        public static T FromJson<T>(this string inParam)
        {
            if (string.IsNullOrWhiteSpace(inParam))
                return default;
            return JsonConvert.DeserializeObject<T>(inParam);
        }
    }
}
