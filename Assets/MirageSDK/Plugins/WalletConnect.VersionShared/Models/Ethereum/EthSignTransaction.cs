using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
    public sealed class EthSignTransaction : JsonRpcRequest
    {
        [JsonProperty("params")]
        private TransactionData[] _parameters;

        [JsonIgnore]
        public TransactionData[] Parameters => _parameters;

        public EthSignTransaction(params TransactionData[] transactionDatas) : base()
        {
            this.Method = "eth_signTransaction";
            this._parameters = transactionDatas;
        }
    }
}