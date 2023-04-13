using Newtonsoft.Json;

namespace MirageSDK.Data
{
    public class NativeCurrency
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("symbol")]
        public string Symbol;
        [JsonProperty("decimals")]
        public int Decimals;
    }
}