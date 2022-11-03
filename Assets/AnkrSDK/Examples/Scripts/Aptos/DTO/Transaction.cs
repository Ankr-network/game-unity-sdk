using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	public class Transaction
	{
		[JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Type;
		[JsonProperty(PropertyName = "hash", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Hash;
		[JsonProperty(PropertyName = "sender", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Sender;
		[JsonProperty(PropertyName = "sequence_number", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public UInt64 SequenceNumber;
		[JsonProperty(PropertyName = "max_gas_amount", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public UInt64 MaxGasAmount;
		[JsonProperty(PropertyName = "gas_unit_price", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public UInt64 GasUnitPrice;
		[JsonProperty(PropertyName = "expiration_timestamp_secs", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public UInt64 ExpirationTimestampSecs;
		[JsonProperty(PropertyName = "payload")]
		public TransactionPayloadDTO Payload;
		[JsonProperty(PropertyName = "signature")]
		public TransactionSignature Signature;
	}
}