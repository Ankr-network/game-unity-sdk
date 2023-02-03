using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.Wallet
{
	[Serializable]
	public class TokenInfo
	{
		[JsonProperty(PropertyName = "tokenId")]
		public string TokenId;
		[JsonProperty(PropertyName = "tokenCode")]
		public string TokenCode;
		[JsonProperty(PropertyName = "amount")]
		public string Amount;
	}
}