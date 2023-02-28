using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models
{
    public class InternalEvent
    {
        [JsonProperty("event")]
        public string @event;
    }
}