using System;
using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.SilentSigning
{
	[RpcMethod("wallet_disconnectSilentSign")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 3003)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 3004)]
	public class SilentSigningDisconnectRequestData : RpcRequestListDataBase
	{
		[Serializable]
		private class SilentSigningDisconnectRequestParams
		{
			[JsonProperty("secret")] public string Secret;
		}
		
		public SilentSigningDisconnectRequestData(string secretToken)
		{
			Add(new SilentSigningDisconnectRequestParams
			{
				Secret = secretToken
			});
		}
	}
}