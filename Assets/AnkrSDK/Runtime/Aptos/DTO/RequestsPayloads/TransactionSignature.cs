using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public abstract class TransactionSignature
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}