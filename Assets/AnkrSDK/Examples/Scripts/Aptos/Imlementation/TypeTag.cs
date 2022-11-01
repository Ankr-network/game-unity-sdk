using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class TypeTag : SerializableAbiPart
	{
		public void Serialize(Serializer serializer)
		{
			throw new System.NotImplementedException();
		}

		public static SerializableAbiPart Deserialize(Deserializer deserializer)
		{
			var index = deserializer.DeserializeUleb128AsUint32();
			
		}
	}

	public class TypeTagBool : SerializableAbiPart
	{
		public void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(0);
		}

		public static SerializableAbiPart Deserialize(Deserializer deserializer)
		{
			return new TypeTagBool();
		}
	}
	
	public class TypeTagU8 : SerializableAbiPart
	{
		public void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(1);
		}

		public static SerializableAbiPart Deserialize(Deserializer deserializer)
		{
			return new TypeTagBool();
		}
	}
}