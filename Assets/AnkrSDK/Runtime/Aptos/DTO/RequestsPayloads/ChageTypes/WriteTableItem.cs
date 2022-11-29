using AnkrSDK.Runtime.Aptos.DTO.RequestsPayloads.ChageTypes;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO.ChageTypes
{
	public class WriteTableItem : WriteSetChange
	{
		[JsonProperty(PropertyName = "state_key_hash")]
		public string StateKeyHash;
		[JsonProperty(PropertyName = "handle")]
		public string Handle;
		[JsonProperty(PropertyName = "key")]
		public string Key;
		[JsonProperty(PropertyName = "value")]
		public string Value;
		[JsonProperty(PropertyName = "data")]
		public DecodedTableData Data;
	}
}