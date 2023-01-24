using System;
using System.Numerics;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.Wallet
{
	[Serializable]
	public class NftInfo
	{
		[JsonProperty(PropertyName = "nftAddress")]
		public string NftAddress;

		[JsonProperty(PropertyName = "tokenId")]
		public BigInteger TokenId;
	}
}