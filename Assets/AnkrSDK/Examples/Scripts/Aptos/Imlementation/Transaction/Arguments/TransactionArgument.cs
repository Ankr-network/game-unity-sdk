using System;
using System.Numerics;
using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public abstract class TransactionArgument : SerializableAbiPart<TransactionArgument>
	{
		public abstract void Serialize(Serializer serializer);

		public static TransactionArgument Deserialize(Deserializer deserializer)
		{
			var index = deserializer.DeserializeUleb128AsUint32();
			switch (index) {
				case 0:
					return TransactionArgumentU8.Load(deserializer);
				case 1:
					return TransactionArgumentU64.Load(deserializer);
				case 2:
					return TransactionArgumentU128.Load(deserializer);
				case 3:
					return TransactionArgumentAddress.Load(deserializer);
				case 4:
					return TransactionArgumentU8Vector.Load(deserializer);
				case 5:
					return TransactionArgumentBool.Load(deserializer);
				default:
					throw new Exception($"Unknown variant index for TransactionArgument: ${index}");
			}
		}
	}
}