
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect2.RpcResponses
{
	public class EthResponseData : RpcResponseBase
	{
		[JsonProperty("result")]
		public string result;

		[JsonIgnore]
		public string Result => result;
	}
}