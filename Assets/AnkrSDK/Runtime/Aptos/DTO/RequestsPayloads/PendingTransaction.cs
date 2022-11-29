using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class PendingTransaction : SubmitTransactionRequest
	{
		[JsonProperty(PropertyName = "hash")]
		public string Hash;
	}
}