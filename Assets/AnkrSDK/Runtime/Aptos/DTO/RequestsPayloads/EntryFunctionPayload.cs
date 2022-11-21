using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class EntryFunctionPayload : TransactionPayload
	{
		[JsonProperty(PropertyName = "function")]
		public string Function;
		[JsonProperty(PropertyName = "type_arguments")]
		public string[] TypeArguments;
		[JsonProperty(PropertyName = "arguments")]
		public string[] Arguments;
	}
}