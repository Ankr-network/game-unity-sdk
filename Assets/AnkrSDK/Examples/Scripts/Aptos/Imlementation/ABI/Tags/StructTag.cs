using System;
using System.Linq;
using AnkrSDK.Aptos.Utils;

namespace AnkrSDK.Aptos.Imlementation
{
	public class StructTag : TypeTag
	{
		private const string Delimiter = "::";
		public byte[] Address { get; private set; }
		public string ModuleName { get; private set; }
		public string Name { get; private set; }
		public TypeTag[] TypeArgs { get; private set; }

		public StructTag(byte[] address, string moduleName, string name, TypeTag[] typeArgs)
		{
			Address = address;
			ModuleName = moduleName;
			Name = name;
			TypeArgs = typeArgs;
		}

		static StructTag FromString(string structTag)
		{
			var parts = structTag.Split(Delimiter);
			if (parts.Length != 3)
			{
				throw new Exception("Invalid struct tag string literal");
			}

			return new StructTag(parts[0].HexToByteArray((int)TypeLength.Address), parts[1], parts[2], Array.Empty<TypeTag>());
		}
        
		public override void Serialize(Serializer serializer) {
			serializer.SerializeFixedBytes(Address);
			serializer.SerializeString(ModuleName);
			serializer.SerializeString(Name);
			serializer.SerializeVector(TypeArgs);
		}

		public static StructTag Deserialize(Deserializer deserializer) {
			var address = deserializer.DeserializeFixedBytes((int)TypeLength.Address);
			var moduleName = deserializer.DeserializeString();
			var name = deserializer.DeserializeString();
			var typeArgs = deserializer.DeserializeVector(TypeTag.Deserialize);
			return new StructTag(address, moduleName, name, typeArgs.ToArray());
		}
	}
}