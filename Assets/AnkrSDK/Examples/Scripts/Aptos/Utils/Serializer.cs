using System;
using System.Collections.Generic;
using System.Numerics;
using AnkrSDK.Aptos.Infrastructure;

namespace AnkrSDK.Aptos.Utils
{
	public class Serializer
	{
		private readonly List<byte> _buffer;
		private int _offset = 0;

		public Serializer()
		{
			_buffer = new List<byte>();
		}

		public void SerializeBytes(IEnumerable<byte> bytes)
		{
			var value = SerializeUtils.SerializeBytes(bytes);
			Add(value);
		}
		public void SerializeUInt32AsUleb128(UInt32 value)
		{
			var bytes = SerializeUtils.SerializeUInt32AsUleb128(value);
			Add(bytes);
		}

		public void SerializeString(string value)
		{
			var bytes = SerializeUtils.SerializeString(value);
			Add(bytes);
		}
		
		public void SerializeBool(bool value)
		{
			var bytes = SerializeUtils.SerializeBool(value);
			Add(bytes);
		}

		public void SerializeUint8(uint value)
		{
			var bytes = SerializeUtils.SerializeUint8(value);
			Add(bytes);
		}
		
		public void SerializeUint16(uint value)
		{
			var bytes = SerializeUtils.SerializeUint16(value);
			Add(bytes);
		}
		
		public void SerializeUin32(uint value)
		{
			var bytes = SerializeUtils.SerializeUin32(value);
			Add(bytes);
		}
		
		public void SerializeUint64(ulong value)
		{
			var bytes = SerializeUtils.SerializeUint64(value);
			Add(bytes);
		}
		
		public void SerializeUint128(string value)
		{
			var bytes = SerializeUtils.SerializeUint128(value);
			Add(bytes);
		}
		
		public void SerializeUInt128(BigInteger value)
		{
			var bytes = SerializeUtils.SerializeUint128(value);
			Add(bytes);		
		}
		
		public void SerializeVector<T>(T[] vector) where T : SerializableAbiPart<T>
		{
			SerializeUInt32AsUleb128((uint)vector.Length);

			foreach (var item in vector)
			{
				item.Serialize(this);
			}
		}

		private void Add(IEnumerable<byte> bytes)
		{
			_buffer.AddRange(bytes);
		}
	}
}