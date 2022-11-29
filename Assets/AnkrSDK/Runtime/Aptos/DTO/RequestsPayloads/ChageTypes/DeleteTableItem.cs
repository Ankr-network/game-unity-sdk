using AnkrSDK.Runtime.Aptos.DTO.RequestsPayloads.ChageTypes;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO.ChageTypes
{
	public class DeleteTableItem : WriteSetChange
	{
		[JsonProperty(PropertyName = "state_key_hash")]
		public string StateKeyHash;
		[JsonProperty(PropertyName = "handle")]
		public string Handle;
		[JsonProperty(PropertyName = "key")]
		public string Key;
		[JsonProperty(PropertyName = "data")]
		public DeletedTableData Data;
	}
}