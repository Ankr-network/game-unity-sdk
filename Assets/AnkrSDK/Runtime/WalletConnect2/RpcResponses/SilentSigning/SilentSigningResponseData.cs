using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect2.RpcResponses.SilentSigning
{
	public class SilentSigningResponseData : RpcResponseBase
	{
		[JsonProperty("result")]
		private string _result;

		[JsonIgnore]
		public string Result => _result;
	}
}