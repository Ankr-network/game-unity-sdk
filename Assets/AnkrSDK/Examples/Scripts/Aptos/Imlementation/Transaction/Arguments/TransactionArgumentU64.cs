using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionArgumentU64 : TransactionArgument
	{
		public readonly ulong Value;

		public TransactionArgumentU64(ulong value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(1);
			serializer.SerializeUint64(Value);
		}

		public static TransactionArgumentU64 Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeUInt64();
			return new TransactionArgumentU64(value);
		}
	}
}