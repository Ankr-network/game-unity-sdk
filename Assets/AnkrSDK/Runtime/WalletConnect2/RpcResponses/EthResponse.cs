
using Newtonsoft.Json;

namespace AnkrSDK.Runtime.WalletConnect2.RpcResponses
{
	public class EthResponseData : RpcResponseBase
	{
		[JsonProperty("result")]
		public string result;

		[JsonIgnore]
		public string Result => result;
	}
}