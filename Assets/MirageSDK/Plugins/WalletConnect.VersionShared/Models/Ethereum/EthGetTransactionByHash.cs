using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetTransactionByHash : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthGetTransactionByHash(string transactionHash) : base()
		{
			this.Method = "eth_getTransactionByHash";
			this._parameters = new object[]
			{
				transactionHash
			};
		}
	}
}