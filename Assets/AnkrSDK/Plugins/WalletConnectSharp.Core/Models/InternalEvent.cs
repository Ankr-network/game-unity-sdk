using Newtonsoft.Json;

namespace AnkrSDK.Plugins.WalletConnectSharp.Core.Models
{
    public class InternalEvent
    {
        [JsonProperty("event")]
        public string @event;
    }
}