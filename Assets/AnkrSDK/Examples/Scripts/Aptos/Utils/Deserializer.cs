using System;
using System.Collections.Generic;
using System.Numerics;

namespace AnkrSDK.Aptos.Utils
{
	public class Deserializer
	{
		private readonly byte[] _buffer;
		private int _offset = 0;

		public Deserializer(byte[] value)
		{
			_buffer = value;
		}
		
		public string DeserializeString()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeString(_buffer, _offset);
			_offset = newOffset;
			return value;
		}
		
		public byte[] DeserializeBytes()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeBytes(_buffer, _offset);
			_offset = newOffset;
			return value;
		}
		
		public byte[] DeserializeFixedBytes(int length)
		{
			var (value, newOffset) = DeserializeUtils.DeserializeFixedBytes(_buffer, length, _offset);
			_offset = newOffset;
			return value;
		}
		
		public int DeserializeUleb128AsUint32()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeUleb128AsUint32(_buffer, _offset);
			_offset = newOffset;
			return value;
		}

		public int DeserializeUInt8()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeUInt8(_buffer, _offset);
			_offset = newOffset;
			return value;
		}

		public UInt64 DeserializeUInt64()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeUInt64(_buffer, _offset);
			_offset = newOffset;
			return value;
		}
		
		public UInt32 DeserializeUInt32()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeUInt32(_buffer, _offset);
			_offset = newOffset;
			return value;
		}
		
		public UInt16 DeserializeUInt16()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeUInt16(_buffer, _offset);
			_offset = newOffset;
			return value;
		}
		
		public BigInteger DeserializeUInt128()
		{
			var (value, newOffset) = DeserializeUtils.DeserializeUInt128(_buffer, _offset);
			_offset = newOffset;
			return value;
		}

		public IEnumerable<T> DeserializeVector<T>(Func<Deserializer, T> deserializeAction)
		{
			var lenght = DeserializeUleb128AsUint32();

			for (var i = 0; i < lenght; i++)
			{
				yield return deserializeAction(this);
			}
		}
	}
}