using System;
using AnkrSDK.Aptos.Converters;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class SubmitTransactionRequest
	{
		[JsonProperty(PropertyName = "sender")]
		public string Sender;
		[JsonProperty(PropertyName = "sequence_number")]
		public string SequenceNumber;
		[JsonProperty(PropertyName = "max_gas_amount")]
		public string MaxGasAmount;
		[JsonProperty(PropertyName = "gas_unit_price")]
		public string GasUnitPrice;
		[JsonProperty(PropertyName = "expiration_timestamp_secs")]
		public string ExpirationTimestampSecs;
		[JsonProperty(PropertyName = "payload")]
		[JsonConverter(typeof(TransactionPayloadConverter))]
		public TransactionPayload Payload;
		[JsonProperty(PropertyName = "signature")]
		[JsonConverter(typeof(TransactionSignatureConverter))]
		public TransactionSignature Signature;
	}
}