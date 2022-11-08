using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionArgumentAddress : TransactionArgument
	{
		public readonly byte[] Value;

		public TransactionArgumentAddress(byte[] value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(3);
			serializer.SerializeFixedBytes(Value);
		}

		public static TransactionArgumentAddress Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeFixedBytes((int)TypeLength.Address);
			return new TransactionArgumentAddress(value);
		}
	}
}