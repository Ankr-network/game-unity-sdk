using AnkrSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.SilentSigning.Data.Responses
{
	public class SilentSigningResponse : JsonRpcResponse
	{
		[JsonProperty("result")] private string _result;

		[JsonIgnore] public string Result => _result;
	}
}