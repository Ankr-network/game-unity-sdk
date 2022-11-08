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

	public class TypeTagBool : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Bool);
		}

		public static TypeTag Load(Deserializer deserializer)
		{
			return new TypeTagBool();
		}
	}
	
	public class TypeTagU8 : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.U8);
		}

		public static TypeTagU8 Load(Deserializer deserializer)
		{
			return new TypeTagU8();
		}
	}
	
	public class TypeTagU64 : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.U64);
		}

		public static TypeTagU64 Load(Deserializer deserializer)
		{
			return new TypeTagU64();
		}
	}
	
	public class TypeTagU128 : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.U128);
		}

		public static TypeTagU128 Load(Deserializer deserializer)
		{
			return new TypeTagU128();
		}
	}
	
	public class TypeTagAddress : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Address);
		}

		public static TypeTagAddress Load(Deserializer deserializer)
		{
			return new TypeTagAddress();
		}
	}
	
	public class TypeTagSigner : TypeTag
	{
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Signer);
		}

		public static TypeTagSigner Load(Deserializer deserializer)
		{
			return new TypeTagSigner();
		}
	}
	
	public class TypeTagVector : TypeTag
	{
		public TypeTag Value { get; private set; }

		public TypeTagVector(TypeTag value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Vector);
			Value.Serialize(serializer);
		}

		public static TypeTagVector Load(Deserializer deserializer)
		{
			var value = Deserialize(deserializer);
			return new TypeTagVector(value);
		}
	}
	
	public class TypeTagStruct : TypeTag
	{
		public StructTag Value { get; private set; }

		public TypeTagStruct(StructTag value)
		{
			Value = value;
		}
		
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128((int)TypeIndex.Struct);
			this.Value.Serialize(serializer);
		}

		public static TypeTagStruct Load(Deserializer deserializer)
		{
			var value = StructTag.Deserialize(deserializer);
			return new TypeTagStruct(value);
		}
	}

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