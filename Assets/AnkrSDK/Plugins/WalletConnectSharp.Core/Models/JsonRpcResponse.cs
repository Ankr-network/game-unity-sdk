using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models
{
    public class JsonRpcResponse : IEventSource
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
            [JsonProperty]
            private int? code;
            
            [JsonProperty]
            private string message;

            [JsonIgnore]
            public int? Code => code;

            [JsonIgnore]
            public string Message => message;
        }

        [JsonIgnore]
        public string Event => "response:" + ID;
    }
}