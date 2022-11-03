using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class RawTransaction : SerializableAbiPart<RawTransaction>
	{
		public readonly byte[] Sender;
		public readonly ulong SequenceNumber;
		public readonly TransactionPayload Payload;
		public readonly ulong MaxGasAmount;
		public readonly ulong GasUnitPrice;
		public readonly ulong ExpirationTimestampSecs;
		public readonly uint ChainId;

		public RawTransaction(
			byte[] sender,
			ulong sequenceNumber,
			TransactionPayload payload,
			ulong maxGasAmount,
			ulong gasUnitPrice,
			ulong expirationTimestampSecs,
			uint chainId)
		{
			Sender = sender;
			SequenceNumber = sequenceNumber;
			Payload = payload;
			MaxGasAmount = maxGasAmount;
			GasUnitPrice = gasUnitPrice;
			ExpirationTimestampSecs = expirationTimestampSecs;
			ChainId = chainId;
		}

		public void Serialize(Serializer serializer)
		{
			serializer.SerializeBytes(Sender);
			serializer.SerializeUint64(SequenceNumber);
			Payload.Serialize(serializer);
			serializer.SerializeUint64(MaxGasAmount);
			serializer.SerializeUint64(GasUnitPrice);
			serializer.SerializeUint64(ExpirationTimestampSecs);
			serializer.SerializeUint8(ChainId);
		}

		private static RawTransaction Deserialize(Deserializer deserializer)
		{
			var sender = deserializer.DeserializeBytes();
			var sequenceNumber = deserializer.DeserializeUInt64();
			var payload = TransactionPayload.Deserialize(deserializer);
			var maxGasAmount = deserializer.DeserializeUInt64();
			var gasUnitPrice = deserializer.DeserializeUInt64();
			var expirationTimestampSecs = deserializer.DeserializeUInt64();
			var chainId = (uint)deserializer.DeserializeUInt8();

			return new RawTransaction(sender, sequenceNumber, payload, maxGasAmount, gasUnitPrice, expirationTimestampSecs, chainId);
		}
	}
}