using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class WriteSet
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}