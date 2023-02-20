using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.TransferNFT
{
	[Serializable]
	public class JsonResponseBase
	{
		[JsonProperty(PropertyName = "error", NullValueHandling = NullValueHandling.Ignore)]
		public string Error;

		[JsonProperty(PropertyName = "code", NullValueHandling = NullValueHandling.Ignore)]
		public long Code;

		public bool IsResponseOk => string.IsNullOrEmpty(Error);
	}
}