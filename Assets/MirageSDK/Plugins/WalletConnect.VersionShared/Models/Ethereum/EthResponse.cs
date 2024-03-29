using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
    public class EthResponse : JsonRpcResponse
    {
        [JsonProperty]
        public string result;

        [JsonIgnore]
        public string Result => result;
    }
}