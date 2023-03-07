using System;
using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.SilentSigning
{
	[RpcMethod("wallet_requestSilentSign")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 3001)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 3002)]
	public class SilentSigningConnectionRequestData : RpcRequestListDataBase
	{
		[Serializable]
		private class SilentSigningConnectionRequestParams
		{
			[JsonProperty("until")] public long Timestamp;

			[JsonProperty("chainId", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public long ChainID;
		}

		public SilentSigningConnectionRequestData(long timestamp, long chainID = 1)
		{
			Add(new SilentSigningConnectionRequestParams
			{
				Timestamp = timestamp,
				ChainID = chainID
			});
		}
	}
}