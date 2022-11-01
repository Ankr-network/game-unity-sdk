using System;
using System.Numerics;
using System.Text;
using Nethereum.Util;

namespace AnkrSDK.Aptos.Utils
{
	public static class DeserializeUtils
	{
		public static (string, int) DeserializeString(byte[] value, int offset)
		{
			var (str, localOffset) = DeserializeBytes(value, offset);
			return (Encoding.ASCII.GetString(str), localOffset);
		}
		
		public static (byte[], int) DeserializeBytes(byte[] value, int offset)
		{
			var (length, prefixOffset) = DeserializeUleb128AsUint32(value, offset);
			var localOffset = offset + prefixOffset + length;
			var bytes = value.Slice(offset + prefixOffset, localOffset);
			return (bytes, localOffset);
		}
		
		public static (int, int) DeserializeUleb128AsUint32(byte[] value, int offset)
		{
			var buf = 0;
			var shift = 0;
			var localOffset = 0;

			while (buf < UInt32.MaxValue)
			{
				var currentByte = value[localOffset + offset];
				buf |= (currentByte & 0x7f) << shift;
				
				localOffset++;
				shift += 7;
				
				if((currentByte & 0x80) == 0)
				{
					break;
				}
			}

			if (buf > UInt32.MaxValue)
			{
				throw new Exception("Overflow while parsing uleb128-encoded uint32 value");
			}

			return (buf, localOffset + offset);
		}

		public static (int, int) DeserializeUInt8(byte[] value, int offset)
		{
			return (value[0], offset + 1);
		}

		public static (UInt64, int) DeserializeUInt64(byte[] value, int offset)
		{
			var (buf, localOffset) = DeserializeUIntN(value, offset, TypeLength.UInt64);
			return ((UInt64)buf, localOffset);
		}
		
		public static (UInt32, int) DeserializeUInt32(byte[] value, int offset)
		{
			var (buf, localOffset) = DeserializeUIntN(value, offset, TypeLength.UInt32);
			return ((UInt32)buf, localOffset);
		}
		
		public static (UInt16, int) DeserializeUInt16(byte[] value, int offset)
		{
			var (buf, localOffset) = DeserializeUIntN(value, offset, TypeLength.UInt16);
			return ((UInt16)buf, localOffset);
		}
		
		public static (BigInteger, int) DeserializeUInt128(byte[] value, int offset)
		{
			var num = new BigInteger();
			for (int i = 0; i < (int)TypeLength.UInt128; i++)
			{
				num |= value[offset + i] << i;
			}

			return (num, offset + (int)TypeLength.UInt128);		
		}
		
		private static (int, int) DeserializeUIntN(byte[] value, int offset, TypeLength typeLength)
		{
			var num = 0;
			for (int i = 0; i < (int)typeLength; i++)
			{
				num |= value[offset + i] << i;
			}

			return (num, offset + (int)typeLength);
		}
	}
}