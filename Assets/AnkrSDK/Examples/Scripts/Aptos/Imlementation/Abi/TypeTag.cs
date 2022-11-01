using System.Collections.Generic;
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
			
			switch (index) {
				case 0:
					return TypeTagBool.load(deserializer);
				case 1:
					return TypeTagU8.load(deserializer);
				case 2:
					return TypeTagU64.load(deserializer);
				case 3:
					return TypeTagU128.load(deserializer);
				case 4:
					return TypeTagAddress.load(deserializer);
				case 5:
					return TypeTagSigner.load(deserializer);
				case 6:
					return TypeTagVector.load(deserializer);
				case 7:
					return TypeTagStruct.load(deserializer);
				default:
					throw new Error(`Unknown variant index for TypeTag: ${index}`);
			}
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

	public class StructTag
	{
		public object Address;
        public Identifier ModuleName;
        public Identifier Name;
        public List<TypeTag> TypeArgs;
	}
}