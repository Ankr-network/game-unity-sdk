using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveFunctionGenericTypeParam
	{
		[JsonProperty(PropertyName = "constraints")]
		public string Constraints;
	}
}