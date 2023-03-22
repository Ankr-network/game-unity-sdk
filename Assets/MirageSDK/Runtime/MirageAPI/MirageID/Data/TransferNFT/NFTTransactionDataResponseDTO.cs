using System;
using Newtonsoft.Json;

namespace MirageSDK.MirageAPI.MirageID.Data.TransferNFT
{
	[Serializable]
	public class NFTTransactionDataResponseDTO : JsonResponseBase
	{
		[JsonProperty(PropertyName = "data")]
		public string Data;
	}
}