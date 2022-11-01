using System;
using AnkrSDK.Aptos.Infrastructure;

namespace AnkrSDK.Aptos.Utils
{
	public class ScriptAbiUtils
	{
		public static SerializableAbiPart Deserialize(Deserializer deserializer)
		{
			var index = deserializer.DeserializeUleb128AsUint32();
			switch (index)
			{
				case 0:
					return;
				case 1:
					return;
				default:
					throw new Exception($"Unknown variant index for TransactionPayload: {index}");
			}
		} 
	}
}