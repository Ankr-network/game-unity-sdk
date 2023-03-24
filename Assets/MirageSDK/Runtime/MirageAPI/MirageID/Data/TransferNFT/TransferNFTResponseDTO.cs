using System;
using Newtonsoft.Json;

namespace MirageSDK.MirageAPI.MirageID.Data.TransferNFT
{
	[Serializable]
	public class TransferNFTResponseDTO : JsonResponseBase
	{
		[JsonProperty(PropertyName = "id")]
		public string TransactionId;
		[JsonProperty(PropertyName = "toWalletId")]
		public string ToWalletId;
		[JsonProperty(PropertyName = "nftId")]
		public string NFTId;
	}
}