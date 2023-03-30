using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnect.VersionShared.Utils;
using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models
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
        
        public override string ToString()
        {
            var jsonStr = JsonConvert.SerializeObject(this);
            return jsonStr;
        }
    }
}