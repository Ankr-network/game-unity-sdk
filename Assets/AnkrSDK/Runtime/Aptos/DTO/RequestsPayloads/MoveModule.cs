using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveModule
	{
		[JsonProperty(PropertyName = "address")]
		public string Address;
		[JsonProperty(PropertyName = "name")]
		public string Name;
		[JsonProperty(PropertyName = "friends")]
		public string[] Friends;
		[JsonProperty(PropertyName = "exposed_functions")]
		public MoveFunction[] ExposedFunctions;
		[JsonProperty(PropertyName = "structs")]
		public MoveStruct[] Structs;
	}
}