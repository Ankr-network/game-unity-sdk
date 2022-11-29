using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveStructGenericTypeParam
	{
		[JsonProperty(PropertyName = "constraints")]
		public string[] Constraints;
	}
}