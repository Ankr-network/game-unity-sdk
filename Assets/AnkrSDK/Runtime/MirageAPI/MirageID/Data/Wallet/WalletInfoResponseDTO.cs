using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.Wallet
{
	[Serializable]
	public class WalletInfoResponseDTO
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "ownerId")]
		public string OwnerId;
		[JsonProperty(PropertyName = "tokenBalance")]
		public TokenInfo[] TokenBalance;
		[JsonProperty(PropertyName = "nftBalance")]
		public NftInfo[] NftBalance;
	}
}