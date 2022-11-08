using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionArgumentBool : TransactionArgument
	{
		public readonly bool Value;

		public TransactionArgumentBool(bool value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(5);
			serializer.SerializeBool(Value);
		}

		public static TransactionArgumentBool Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeBool();
			return new TransactionArgumentBool(value);
		}
	}
}