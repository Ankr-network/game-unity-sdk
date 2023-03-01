using AnkrSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models
{
    public class WCSessionUpdate : JsonRpcRequest
    {
        public const string SessionUpdateMethod = "wc_sessionUpdate";

        [JsonProperty("params")]
        public WCSessionData[] parameters;

        public WCSessionUpdate(WCSessionData data)
        {
            Method = SessionUpdateMethod;
            this.parameters = new[] {data};
        }
    }
}