using AnkrSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events.Model.Ethereum
{
    public sealed class EthGenericRequest<T> : JsonRpcRequest
    {
        [JsonProperty("params")] 
        private T[] _parameters;

        [JsonIgnore]
        public T[] Parameters => _parameters;

        public EthGenericRequest(string jsonRpcMethodName, params T[] data) : base()
        {
            this.Method = jsonRpcMethodName;
            this._parameters = data;
        }
    }
}