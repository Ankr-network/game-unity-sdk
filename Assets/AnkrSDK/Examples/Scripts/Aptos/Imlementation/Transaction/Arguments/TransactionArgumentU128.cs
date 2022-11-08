using System.Numerics;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public class TransactionArgumentU128 : TransactionArgument
	{
		public readonly BigInteger Value;

		public TransactionArgumentU128(BigInteger value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(2);
			serializer.SerializeUint128(Value);
		}

		public static TransactionArgumentU128 Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeUInt128();
			return new TransactionArgumentU128(value);
		}
	}
}