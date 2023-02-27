using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.Runtime.WalletConnect2.RpcResponses
{
	[RpcResponseOptions(Clock.ONE_DAY, false, 1108)]
	public class RpcResponseBase
	{
		[JsonProperty]
		private JsonRpcError error;

		[JsonIgnore]
		public JsonRpcError Error => error;
	}
}