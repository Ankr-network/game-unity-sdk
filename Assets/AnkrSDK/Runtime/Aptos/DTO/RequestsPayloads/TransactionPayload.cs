using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class TransactionPayload
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}