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
			var index = (ScriptAbiIndex)deserializer.DeserializeUleb128AsUint32();
			switch (index)
			{
				case ScriptAbiIndex.TransactionScript:
					return TransactionScriptABI.Load(deserializer);
				case ScriptAbiIndex.EntryFunction:
					return EntryFunctionABI.Load(deserializer);
				default:
					throw new Exception($"Unknown variant index for TransactionPayload: {index}");
			}
		} 
	}
}