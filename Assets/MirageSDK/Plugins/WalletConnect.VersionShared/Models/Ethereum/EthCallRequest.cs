using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthCallRequest : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthCallRequest(TransactionData transactionData) : base()
		{
			this.Method = "eth_call";
			this._parameters = new object[]
			{
				transactionData,
				"latest",
			};
		}
	}
}