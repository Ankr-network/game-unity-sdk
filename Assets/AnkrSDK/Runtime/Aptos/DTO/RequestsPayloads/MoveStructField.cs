using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveStructField
	{
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}