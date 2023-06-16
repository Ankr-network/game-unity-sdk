using MirageSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO.JsonRpc
{
	public class EthResponse : JsonRpcResponse
	{
		[JsonProperty]
		public string result;

		[JsonIgnore]
		public string Result => result;
	}
}