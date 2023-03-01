using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnect.VersionShared.Models;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcResponses
{
	[RpcResponseOptions(Clock.ONE_DAY, false, 1108)]
	public class RpcResponseBase : IErrorHolder
	{
		//these properties are only required for WC and WC2 to have a unified interface, 
		//for WC2 response errors are handled by internal WalletConnectSharp request engine
		public JsonRpcResponse.JsonRpcError Error => null;
		public bool IsError => false;
	}
}