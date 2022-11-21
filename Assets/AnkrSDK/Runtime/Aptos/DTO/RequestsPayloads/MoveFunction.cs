using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveFunction
	{
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "visibility")]
		public string Visibility;
		[JsonProperty(PropertyName = "is_entry")]
		public bool IsEntry;
		[JsonProperty(PropertyName = "generic_type_params")]
		public MoveFunctionGenericTypeParam[] GenericTypeParams;
		[JsonProperty(PropertyName = "params")]
		public string[] Params;
		[JsonProperty(PropertyName = "return")]
		public string[] Return;
	}
}