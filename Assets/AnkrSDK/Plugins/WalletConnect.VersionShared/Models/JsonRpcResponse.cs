using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect.VersionShared.Models
{
    public class JsonRpcResponse : IEventSource, IErrorHolder
    {
        [JsonProperty]
        private long id;
        
        [JsonProperty]
        private string jsonrpc = "2.0";

        [JsonProperty]
        private JsonRpcError error;

        [JsonIgnore]
        public JsonRpcError Error => error;

        [JsonIgnore]
        public bool IsError => error != null;

        [JsonIgnore]
        public long ID => id;

        [JsonIgnore]
        public string JsonRPC => jsonrpc;

        public class JsonRpcError
        {
            [JsonProperty("code")]
            private int? code;
            
            [JsonProperty("message")]
            private string message;

            [JsonProperty("data")] 
            private string data;

            [JsonIgnore]
            public int? Code => code;

            [JsonIgnore]
            public string Message => message;

            [JsonIgnore] 
            public string Data => data;
            
            public JsonRpcError ()
            {
                
            }
        }

        [JsonIgnore]
        public string Event => "response:" + ID;
    }
}