using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.Token
{
	[Serializable]
	public class TokenResponseDTO
	{
		[JsonProperty(PropertyName = "access_token")]
		public string AccessToken;

		[JsonProperty(PropertyName = "expires_in")]
		public long ExpiresIn;
	}
}