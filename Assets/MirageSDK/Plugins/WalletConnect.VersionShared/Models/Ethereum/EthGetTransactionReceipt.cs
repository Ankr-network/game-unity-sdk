using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetTransactionReceipt : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthGetTransactionReceipt(string transactionHash)
		{
			this.Method = "eth_getTransactionReceipt";
			this._parameters = new object[]
			{
				transactionHash
			};
		}
	}
}