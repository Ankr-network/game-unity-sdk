using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionPayloadScript : TransactionPayload
	{
		private readonly Script Value;

		public TransactionPayloadScript(Script value)
		{
			Value = value;
		}
		
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(0);
			Value.Serialize(serializer);
		}

		public static TransactionPayloadScript Load(Deserializer deserializer)
		{
			var value = Script.Deserialize(deserializer);
			return new TransactionPayloadScript(value);
		}
	}
}