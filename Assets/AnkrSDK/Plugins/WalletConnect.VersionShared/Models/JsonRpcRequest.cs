using AnkrSDK.Plugins.WalletConnect.VersionShared.Utils;
using Newtonsoft.Json;

namespace AnkrSDK.Plugins.WalletConnect.VersionShared.Models
{
    public class JsonRpcRequest : IEventSource
    {
        [JsonProperty]
        protected long id;
        [JsonProperty]
        private string jsonrpc = "2.0";
        
        [JsonProperty("method")]
        public virtual string Method { get; protected set; }

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