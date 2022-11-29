using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class TransactionSignature
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}