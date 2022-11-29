using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class PendingTransaction : SubmitTransactionRequest1
	{
		[JsonProperty(PropertyName = "hash")]
		public string Hash;
	}
}