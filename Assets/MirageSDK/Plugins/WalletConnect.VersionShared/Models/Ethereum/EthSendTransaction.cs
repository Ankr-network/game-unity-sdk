using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
    public sealed class EthSendTransaction : JsonRpcRequest
    {
        [JsonProperty("params")]
        private TransactionData[] _parameters;

        [JsonIgnore]
        public TransactionData[] Parameters => _parameters;

        public EthSendTransaction(params TransactionData[] transactionDatas) : base()
        {
            this.Method = "eth_sendTransaction";
            this._parameters = transactionDatas;
        }
    }
}