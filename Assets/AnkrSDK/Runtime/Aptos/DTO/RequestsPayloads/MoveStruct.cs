using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveStruct
	{
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "is_native")]
		public string IsNative;
		[JsonProperty(PropertyName = "abilities")]
		public string[] Abilities;
		[JsonProperty(PropertyName = "generic_type_params")]
		public MoveStructGenericTypeParam[] GenericTypeParams;
		[JsonProperty(PropertyName = "fields")]
		public MoveStructField[] Fields;
	}
}