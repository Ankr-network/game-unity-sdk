using AnkrSDK.Aptos.Converters;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class WrappedTransaction
	{
		[JsonProperty(PropertyName = "transaction")]
		[JsonConverter(typeof(TransactionConverter))]
		public TypedTransaction Transaction;
	}
}