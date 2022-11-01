using System;
using AnkrSDK.Aptos.Imlementation;
using AnkrSDK.Aptos.Infrastructure;

namespace AnkrSDK.Aptos.Utils
{
	public abstract class ScriptAbi : SerializableAbiPart<ScriptAbi>
	{
		public abstract void Serialize(Serializer serializer);

		public static ScriptAbi Deserialize(Deserializer deserializer)
		{
			var index = deserializer.DeserializeUleb128AsUint32();
			switch (index)
			{
				case 0:
					return TransactionScriptABI.Load(deserializer);
				case 1:
					return EntryFunctionABI.Load(deserializer);
				default:
					throw new Exception($"Unknown variant index for TransactionPayload: {index}");
			}
		} 
	}
}