using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models
{
    public class WCSessionUpdate : JsonRpcRequest
    {
        public const string SessionUpdateMethod = "wc_sessionUpdate";
        public override string Method => SessionUpdateMethod;

        [JsonProperty("params")]
        public WCSessionData[] parameters;

        public WCSessionUpdate(WCSessionData data)
        {
            this.parameters = new[] {data};
        }
    }
}