using System;
using System.Numerics;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.TransferNFT
{
	[Serializable]
	public class CreateNFTTransactionResponseDTO : JsonResponseBase
	{
		[JsonProperty(PropertyName = "id")]
		public string ID;
		[JsonProperty(PropertyName = "toWalletId")]
		public string ToWalletId;
		[JsonProperty(PropertyName = "fromWalletId")]
		public string FromWalletId;
		[JsonProperty(PropertyName = "tokenId")]
		public BigInteger TokenID;
	}
}