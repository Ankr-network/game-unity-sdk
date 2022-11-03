using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class TransactionPayloadDTO
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
		[JsonProperty(PropertyName = "function")]
		public string Function;
		[JsonProperty(PropertyName = "type_arguments")]
		public string[] TypeArguments;
		[JsonProperty(PropertyName = "arguments")]
		public string[] Arguments;
	}
}