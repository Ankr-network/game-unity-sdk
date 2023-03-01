
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect2.RpcResponses.Eth
{
	public class EthResponseData : RpcResponseBase
	{
		[JsonProperty("result")]
		private string _result;

		[JsonIgnore]
		public string Result => _result;
	}
}