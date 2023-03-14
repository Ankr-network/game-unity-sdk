using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.SilentSigning
{
	[RpcMethod("wallet_silentSignMessage")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 3005)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 3006)]
	public class SilentSigningSignMessageRequestData : RpcRequestListDataBase
	{
		private class SilentSigningSignMessageRequestParams
		{
			[JsonProperty("message")] 
			public string Message;

			[JsonProperty("address")]
			public string Address;
		}

		public SilentSigningSignMessageRequestData(string address, string message)
		{
			Add(new SilentSigningSignMessageRequestParams()
			{
				Message = message,
				Address = address
			});
		}
	}
}