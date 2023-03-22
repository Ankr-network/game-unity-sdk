using System;
using Newtonsoft.Json;

namespace MirageSDK.MirageAPI.MirageID.Data.TransferNFT
{
	[Serializable]
	public class CreateNFTTransactionRequestDTO
	{
		[JsonProperty(PropertyName = "transactionHash")]
		public string TransactionHash;
	}
}