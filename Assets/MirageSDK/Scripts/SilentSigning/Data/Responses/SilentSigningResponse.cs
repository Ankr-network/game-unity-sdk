using MirageSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace MirageSDK.SilentSigning.Data.Responses
{
	public class SilentSigningResponse : JsonRpcResponse
	{
		[JsonProperty("result")] private string _result;

		[JsonIgnore] public string Result => _result;
	}
}