using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionPayloadEntryFunction : TransactionPayload
	{
		private readonly EntryFunction Value;

		public TransactionPayloadEntryFunction(EntryFunction value)
		{
			Value = value;
		}
		
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(2);
			Value.Serialize(serializer);
		}

		public static TransactionPayloadEntryFunction Load(Deserializer deserializer)
		{
			var value = EntryFunction.Deserialize(deserializer);
			return new TransactionPayloadEntryFunction(value);
		}
	}
}