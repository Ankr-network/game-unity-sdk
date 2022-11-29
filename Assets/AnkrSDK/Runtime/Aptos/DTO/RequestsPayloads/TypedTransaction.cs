using System;
using AnkrSDK.Aptos.Converters;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class TypedTransaction
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}