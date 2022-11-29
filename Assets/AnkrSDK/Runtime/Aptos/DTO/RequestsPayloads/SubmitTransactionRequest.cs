using System;
using Newtonsoft.Json;

namespace AnkrSDK.Aptos.DTO
{
	[Serializable]
	public class SubmitTransactionRequest
	{
		[JsonProperty(PropertyName = "sender")]
		public string Sender;
		[JsonProperty(PropertyName = "sequence_number")]
		public ulong SequenceNumber;
		[JsonProperty(PropertyName = "max_gas_amount")]
		public ulong MaxGasAmount;
		[JsonProperty(PropertyName = "gas_unit_price")]
		public ulong GasUnitPrice;
		[JsonProperty(PropertyName = "expiration_timestamp_secs")]
		public ulong ExpirationTimestampSecs;
		[JsonProperty(PropertyName = "payload")]
		public TransactionPayload Payload;
		[JsonProperty(PropertyName = "signature")]
		public TransactionSignature Signature;

		[JsonIgnore]
		public uint ChainID;

		public static implicit operator SubmitTransactionRequest1(SubmitTransactionRequest original)
		{
			return new SubmitTransactionRequest1
			{
				Sender = original.Sender,
				SequenceNumber = original.SequenceNumber.ToString(),
				MaxGasAmount = original.MaxGasAmount.ToString(),
				GasUnitPrice = original.GasUnitPrice.ToString(),
				ExpirationTimestampSecs = original.ExpirationTimestampSecs.ToString(),
				Payload = original.Payload,
				Signature = original.Signature,
			};
		}
	}
}