using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class SubmitTransactionRequest1<TPayload, TSignature>
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
		public TPayload Payload;
		[JsonProperty(PropertyName = "signature")]
		public TSignature Signature;
	}
}