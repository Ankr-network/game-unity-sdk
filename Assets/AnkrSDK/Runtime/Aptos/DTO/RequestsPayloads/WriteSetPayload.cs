using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class WriteSetPayload
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
		// TODO Add converter for all WriteSet
		[JsonProperty(PropertyName = "write_set")]
		public WriteSet WriteSet;
	}
}