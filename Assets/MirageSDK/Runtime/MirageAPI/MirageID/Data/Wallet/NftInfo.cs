using System;
using System.Numerics;
using Newtonsoft.Json;

namespace MirageSDK.MirageAPI.MirageID.Data.Wallet
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