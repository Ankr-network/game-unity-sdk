using System;
using System.Collections.Generic;
using System.Linq;

namespace AnkrSDK.Aptos.Utils
{
	public static class ByteUtil
      {
        public static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];
        public static readonly byte[] ZERO_BYTE_ARRAY = new byte[1];
    
        public static byte[] AppendByte(byte[] bytes, byte b)
        {
          byte[] destinationArray = new byte[bytes.Length + 1];
          Array.Copy((Array) bytes, (Array) destinationArray, bytes.Length);
          destinationArray[destinationArray.Length - 1] = b;
          return destinationArray;
        }
    
        public static byte[] Slice(this byte[] org, int start, int end = 2147483647)
        {
          if (end < 0)
            end = org.Length + end;
          start = Math.Max(0, start);
          end = Math.Max(start, end);
          return ((IEnumerable<byte>) org).Skip<byte>(start).Take<byte>(end - start).ToArray<byte>();
        }
    
        public static byte[] InitialiseEmptyByteArray(int length)
        {
          byte[] numArray = new byte[length];
          for (int index = 0; index < length; ++index)
            numArray[index] = (byte) 0;
          return numArray;
        }
    
        public static IEnumerable<byte> MergeToEnum(params byte[][] arrays)
        {
          byte[][] numArray1 = arrays;
          for (int index1 = 0; index1 < numArray1.Length; ++index1)
          {
            byte[] numArray2 = numArray1[index1];
            for (int index2 = 0; index2 < numArray2.Length; ++index2)
              yield return numArray2[index2];
            numArray2 = (byte[]) null;
          }
          numArray1 = (byte[][]) null;
        }
    
        public static byte[] Merge(params byte[][] arrays) => ByteUtil.MergeToEnum(arrays).ToArray<byte>();
    
        public static byte[] XOR(this byte[] a, byte[] b)
        {
          int length = Math.Min(a.Length, b.Length);
          byte[] numArray = new byte[length];
          for (int index = 0; index < length; ++index)
            numArray[index] = (byte) ((uint) a[index] ^ (uint) b[index]);
          return numArray;
        }
      }
}