using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class EventGuid
	{
		[JsonProperty(PropertyName = "creation_number")]
		public string CreationNumber;
		[JsonProperty(PropertyName = "account_address")]
		public string AccountAddress;
	}
}