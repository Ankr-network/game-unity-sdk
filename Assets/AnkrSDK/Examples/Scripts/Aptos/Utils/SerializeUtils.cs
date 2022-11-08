using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AnkrSDK.Aptos.Utils
{
	public static class SerializeUtils
	{
		public static IEnumerable<byte> SerializeBytes(IEnumerable<byte> bytes)
		{
			var serializedBytes = SerializeUInt32AsUleb128((uint)bytes.Count());
			serializedBytes = serializedBytes.Concat(bytes);
			return serializedBytes;
		}
		public static IEnumerable<byte> SerializeUInt32AsUleb128(UInt32 value)
		{
			while (value >> 7 != 0)
			{
				yield return (byte)((value & 0x7f) | 0x80);
				value >>= 7;
			}
			yield return (byte)value;
		}

		public static IEnumerable<byte> SerializeString(string value)
		{
			return SerializeBytes(Encoding.ASCII.GetBytes(value));
		}
		
		public static IEnumerable<byte> SerializeBool(bool value)
		{
			yield return (byte)(value ? 1 : 0);
		}

		public static IEnumerable<byte> SerializeUint8(uint value)
		{
			return SerializeUIntN(value, TypeLength.UInt8);
		}
		
		public static IEnumerable<byte> SerializeUint16(uint value)
		{
			return SerializeUIntN(value, TypeLength.UInt16);
		}
		
		public static IEnumerable<byte> SerializeUin32(uint value)
		{
			return SerializeUIntN(value, TypeLength.UInt32);
		}
		
		public static IEnumerable<byte> SerializeUint64(ulong value)
		{
			return SerializeUIntN(value, TypeLength.UInt64);
		}
		
		public static IEnumerable<byte> SerializeUint128(string value)
		{
			return SerializeUint128(BigInteger.Parse(value));
		}
		
		public static IEnumerable<byte> SerializeUint128(BigInteger value)
		{
			for (int i = 0; i < (int)TypeLength.UInt128; i++)
			{
				yield return unchecked((byte)(value >> 8 * i));
			}			
		}

		private static IEnumerable<byte> SerializeUIntN(ulong value, TypeLength typeLength)
		{
			for (int i = 0; i < (int)typeLength; i++)
			{
				yield return unchecked((byte)(value >> 8 * i));
			}			
		}
	}
}