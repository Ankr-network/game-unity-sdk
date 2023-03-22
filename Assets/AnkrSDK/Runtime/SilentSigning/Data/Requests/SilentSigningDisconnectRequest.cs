using System;
using AnkrSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace AnkrSDK.SilentSigning.Data.Requests
{
	public class SilentSigningDisconnectRequest : JsonRpcRequest
	{
		[Serializable]
		public class SilentSigningDisconnectRequestParams
		{
			[JsonProperty("secret")] public string Secret;
		}

		[JsonProperty("params")] public SilentSigningDisconnectRequestParams[] Params;

		public SilentSigningDisconnectRequest(string secretToken)
		{
			Method = "wallet_disconnectSilentSign";
			Params = new[]
			{
				new SilentSigningDisconnectRequestParams
				{
					Secret = secretToken
				}
			};
		}
	}
}