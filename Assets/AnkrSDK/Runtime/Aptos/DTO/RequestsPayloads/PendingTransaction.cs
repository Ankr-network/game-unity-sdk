using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class PendingTransaction<TPayload, TSignature> : SubmitTransactionRequest<TPayload, TSignature>
	{
		[JsonProperty(PropertyName = "hash")]
		public string Hash;
	}
}