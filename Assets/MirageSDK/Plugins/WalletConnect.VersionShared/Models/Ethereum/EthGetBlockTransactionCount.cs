using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetBlockTransactionCount : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthGetBlockTransactionCount(string blockNumber)
		{
			this.Method = "eth_getBlockTransactionCountByNumber";
			_parameters = new[]
			{
				blockNumber
			};
		}
	}
}