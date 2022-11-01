using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class TransactionSignature
	{
		[JsonProperty(PropertyName = "type")]
		public string Type;
		[JsonProperty(PropertyName = "public_key")]
		public string PublicKey;
		[JsonProperty(PropertyName = "Signature")]
		public string Signature;
	}
}