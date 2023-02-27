using AnkrSDK.Plugins.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.Plugins.WalletConnectSharp.Core.Events.Model.Ethereum
{
    public class EthResponse : JsonRpcResponse
    {
        [JsonProperty]
        public string result;

        [JsonIgnore]
        public string Result => result;
    }
}