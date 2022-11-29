using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO.ChageTypes
{
	public class DeleteResource : WriteSetChange
	{
		[JsonProperty(PropertyName = "address")]
		public string Address;
		[JsonProperty(PropertyName = "state_key_hash")]
		public string StateKeyHash;
		[JsonProperty(PropertyName = "module")]
		public string Module;
	}
}