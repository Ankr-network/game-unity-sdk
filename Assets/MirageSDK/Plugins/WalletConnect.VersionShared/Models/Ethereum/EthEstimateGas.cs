using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthEstimateGas : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;
		public EthEstimateGas(TransactionData transactionData) : base()
		{
			this.Method = "eth_estimateGas";
			this._parameters = new object[]
			{
				transactionData
			};
		}
	}
}