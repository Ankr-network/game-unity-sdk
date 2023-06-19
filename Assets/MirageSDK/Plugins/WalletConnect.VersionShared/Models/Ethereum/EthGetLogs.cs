using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetLogs : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthGetLogs(NewFilterInput filters)
		{
			this.Method = "eth_getLogs";
			this._parameters = new object[]
			{
				filters
			};
		}
	}
}