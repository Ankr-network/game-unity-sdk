using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models.Ethereum
{
    public class EthResponse : JsonRpcResponse
    {
        [JsonProperty]
        private string result;

        [JsonIgnore]
        public string Result => result;
    }
}