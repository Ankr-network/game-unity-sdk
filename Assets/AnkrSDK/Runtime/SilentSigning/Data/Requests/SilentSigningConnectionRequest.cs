using System;
using AnkrSDK.WalletConnectSharp.Core.Models;
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

		public override string Method => "wallet_requestSilentSign";
		[JsonProperty("params")] public SilentSigningConnectionRequestParams[] Params;

		public SilentSigningConnectionRequest(long timestamp, long chainID = 1)
		{
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