using System;
using Newtonsoft.Json;

namespace MirageSDK.MirageAPI.MirageID.Data.TransferNFT
{
	[Serializable]
	public class TransferNFTRequestDTO
	{
		[JsonProperty(PropertyName = "toWalletId")]
		public string ToWalletId;
		[JsonProperty(PropertyName = "nftId")]
		public string NFTId;
		[JsonProperty(PropertyName = "tokenId")]
		public string TokenID;
	}
}