using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class WriteSetChange
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
	}
}