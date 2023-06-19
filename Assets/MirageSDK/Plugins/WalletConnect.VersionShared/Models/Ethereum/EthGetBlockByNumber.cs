using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetBlockByNumber : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthGetBlockByNumber(string blockId, bool transactionDetail) : base()
		{
			this.Method = "eth_getBlockByNumber";
			this._parameters = new object[]
			{
				blockId,
				transactionDetail,
			};
		}

	}
}