using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionArgumentU8Vector : TransactionArgument
	{
		public readonly byte[] Value;

		public TransactionArgumentU8Vector(byte[] value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(4);
			serializer.SerializeBytes(Value);
		}

		public static TransactionArgumentU8Vector Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeBytes();
			return new TransactionArgumentU8Vector(value);
		}
	}
}