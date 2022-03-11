using Newtonsoft.Json;

namespace MirageSDK.WalletConnectSharp.Core.Models
{
    public class InternalEvent
    {
        [JsonProperty("event")]
        public string @event;
    }
}