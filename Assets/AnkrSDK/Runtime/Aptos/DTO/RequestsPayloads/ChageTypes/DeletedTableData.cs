using Newtonsoft.Json;

namespace AnkrSDK.Runtime.Aptos.DTO.RequestsPayloads.ChageTypes
{
	public class DeletedTableData
	{
		[JsonProperty(PropertyName = "key")]
		public object Key;
		[JsonProperty(PropertyName = "key_type")]
		public string KeyType;
	}
}