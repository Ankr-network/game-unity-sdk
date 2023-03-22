using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnect.VersionShared.Utils;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect.VersionShared.Models
{
    public class JsonRpcRequest : IEventSource, IIdentifiable
    {
        [JsonProperty]
        protected long id;
        [JsonProperty]
        private string jsonrpc = "2.0";
        
        [JsonProperty("method")]
        public string Method { get; protected set; }

        public JsonRpcRequest()
        {
            if (this.id == 0)
            {
                this.id = RpcPayloadId.Generate();
            }
        }
        
        [JsonIgnore]
        public long ID => id;

        [JsonIgnore]
        public string JsonRPC => jsonrpc;

        [JsonIgnore]
        public string Event => Method;
    }
}