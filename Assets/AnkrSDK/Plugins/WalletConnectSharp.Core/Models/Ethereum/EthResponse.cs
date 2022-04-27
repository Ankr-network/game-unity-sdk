using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models.Ethereum
{
    public class EthResponse : JsonRpcResponse
    {
        [JsonProperty]
        public string result;

        [JsonIgnore]
        public string Result => result;
    }
}