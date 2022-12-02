using System;
using AnkrSDK.WalletConnectSharp.Core.Models;
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

		public override string Method => "wallet_disconnectSilentSign";

		[JsonProperty("params")] public SilentSigningDisconnectRequestParams[] Params;

		public SilentSigningDisconnectRequest(string secretToken)
		{
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