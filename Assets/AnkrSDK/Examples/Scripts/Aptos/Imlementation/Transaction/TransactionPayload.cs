using System;
using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation.ABI
{
	public abstract class TransactionPayload : SerializableAbiPart<TransactionPayload>
	{
		public abstract void Serialize(Serializer serializer);

		public static TransactionPayload Deserialize(Deserializer deserializer)
		{
			var index = deserializer.DeserializeUleb128AsUint32();
			switch (index)
			{
				case 0:
					return TransactionPayloadScript.Load(deserializer);
				// TODO: change to 1 once ModuleBundle has been removed from rust
				case 2:
					return TransactionPayloadEntryFunction.Load(deserializer);
				default:
					throw new Exception($"Unknown variant index for TransactionPayload: ${index}");
			}
		}
	}
}