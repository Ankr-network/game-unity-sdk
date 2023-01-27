using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.CreateUser
{
	[Serializable]
	public class CreateUserResponseDTO
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "walletId")]
		public string WalletId;
	}
}