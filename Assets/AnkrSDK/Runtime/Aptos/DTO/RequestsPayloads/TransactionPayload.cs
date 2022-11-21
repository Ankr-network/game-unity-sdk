using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public abstract class TransactionPayload
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}