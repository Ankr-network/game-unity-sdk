using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class MoveResource<TData>
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
		[JsonProperty(PropertyName = "data")]
		public TData Data;
	}
}