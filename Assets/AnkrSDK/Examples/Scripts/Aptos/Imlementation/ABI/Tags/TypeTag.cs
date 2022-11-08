using System;
using System.Linq;
using AnkrSDK.Aptos.Infrastructure;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public abstract class TypeTag : SerializableAbiPart<TypeTag>
	{
		public abstract void Serialize(Serializer serializer);

		public static TypeTag Deserialize(Deserializer deserializer)
		{
			var index = (TypeIndex)deserializer.DeserializeUleb128AsUint32();
			
			switch (index) {
				case TypeIndex.Bool:
					return TypeTagBool.Load(deserializer);
				case TypeIndex.U8:
					return TypeTagU8.Load(deserializer);
				case TypeIndex.U64:
					return TypeTagU64.Load(deserializer);
				case TypeIndex.U128:
					return TypeTagU128.Load(deserializer);
				case TypeIndex.Address:
					return TypeTagAddress.Load(deserializer);
				case TypeIndex.Signer:
					return TypeTagSigner.Load(deserializer);
				case TypeIndex.Vector:
					return TypeTagVector.Load(deserializer);
				case TypeIndex.Struct:
					return TypeTagStruct.Load(deserializer);
				default:
					throw new Exception($"Unknown variant index for TypeTag: {index}");
			}
		}
	}
}