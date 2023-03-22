using MirageSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace MirageSDK.WalletConnectSharp.Core.Events.Model.Ethereum
{
    public sealed class EthSign : JsonRpcRequest
    {
        [JsonProperty("params")] 
        private string[] _parameters;

        [JsonIgnore]
        public string[] Parameters => _parameters;

        public EthSign(string address, string hexData) : base()
        {
            this.Method = "eth_sign";
            this._parameters = new[] {address, hexData};
        }
    }
}