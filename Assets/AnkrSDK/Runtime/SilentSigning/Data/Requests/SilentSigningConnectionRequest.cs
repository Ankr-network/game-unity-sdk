using System;
using AnkrSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.SilentSigning.Data.Requests
{
	public class SilentSigningConnectionRequest : JsonRpcRequest
	{
		[Serializable]
		public class SilentSigningConnectionRequestParams
		{
			[JsonProperty("until")] public long Timestamp;

			[JsonProperty("chainId", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public long ChainID;
		}

		[JsonProperty("params")] public SilentSigningConnectionRequestParams[] Params;

		public SilentSigningConnectionRequest(long timestamp, long chainID = 1)
		{
			Method = "wallet_requestSilentSign";
			Params = new[]
			{
				new SilentSigningConnectionRequestParams
				{
					Timestamp = timestamp,
					ChainID = chainID
				}
			};
		}
	}
}