#region

using Newtonsoft.Json;

#endregion

namespace MagService
{
    public static class AppConfig
    {
        public const string PostContentType = "application/json";
        public const string MagApi = "https://oxfordhk.azure-api.net/academic/v1.0/evaluate";
        public const string SubscriptionKey = "f7cc29509a8443c5b3a5e56b0e38b5a6";
        public const int Count = int.MaxValue - 1;
        public const int Timeout = int.MaxValue - 1;

        public static JsonSerializerSettings JsonSerializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; }
        };
    }
}