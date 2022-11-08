using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionArgumentU8 : TransactionArgument
	{
		public readonly uint Value;

		public TransactionArgumentU8(uint value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(0);
			serializer.SerializeUint8(Value);
		}

		public static TransactionArgumentU8 Load(Deserializer deserializer)
		{
			var value = (uint)deserializer.DeserializeUInt8();
			return new TransactionArgumentU8(value);
		}
	}
}