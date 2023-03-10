using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using AnkrSDK.WalletConnect.VersionShared.Models;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcResponses
{
	public class RpcResponseBase : IErrorHolder
	{
		//these properties are required for WC and WC2 to have a unified interface, 
		//for WC2 they are being set within WalletConnect.Sign DLL on WC2 response via reflection
		public JsonRpcResponse.JsonRpcError Error => null;
		public bool IsError => false;
	}
}