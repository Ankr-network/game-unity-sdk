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
	
	public class TransactionArgumentU8 : TransactionArgument
	{
		public readonly uint Value;

		public TransactionArgumentU8(uint value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(0);
			serializer.SerializeUint8(Value);
		}

		public static TransactionArgumentU8 Load(Deserializer deserializer)
		{
			var value = (uint)deserializer.DeserializeUInt8();
			return new TransactionArgumentU8(value);
		}
	}
	
	public class TransactionArgumentU64 : TransactionArgument
	{
		public readonly ulong Value;

		public TransactionArgumentU64(ulong value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(1);
			serializer.SerializeUint64(Value);
		}

		public static TransactionArgumentU64 Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeUInt64();
			return new TransactionArgumentU64(value);
		}
	}
	
	public class TransactionArgumentU128 : TransactionArgument
	{
		public readonly BigInteger Value;

		public TransactionArgumentU128(BigInteger value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(2);
			serializer.SerializeUint128(Value);
		}

		public static TransactionArgumentU128 Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeUInt128();
			return new TransactionArgumentU128(value);
		}
	}
	
	public class TransactionArgumentAddress : TransactionArgument
	{
		public readonly byte[] Value;

		public TransactionArgumentAddress(byte[] value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(3);
			serializer.SerializeFixedBytes(Value);
		}

		public static TransactionArgumentAddress Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeFixedBytes((int)TypeLength.Address);
			return new TransactionArgumentAddress(value);
		}
	}
	
	public class TransactionArgumentU8Vector : TransactionArgument
	{
		public readonly byte[] Value;

		public TransactionArgumentU8Vector(byte[] value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(4);
			serializer.SerializeBytes(Value);
		}

		public static TransactionArgumentU8Vector Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeBytes();
			return new TransactionArgumentU8Vector(value);
		}
	}
	
	public class TransactionArgumentBool : TransactionArgument
	{
		public readonly bool Value;

		public TransactionArgumentBool(bool value)
		{
			Value = value;
		}
		public override void Serialize(Serializer serializer)
		{
			serializer.SerializeUInt32AsUleb128(5);
			serializer.SerializeBool(Value);
		}

		public static TransactionArgumentBool Load(Deserializer deserializer)
		{
			var value = deserializer.DeserializeBool();
			return new TransactionArgumentBool(value);
		}
	}
}