using System;
using System.Collections.Generic;
using System.Numerics;
using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.Imlementation;
using AnkrSDK.Aptos.Imlementation.ABI;

namespace AnkrSDK.Aptos.Utils
{
	public static class BuildUtils
	{
		public static IEnumerable<Token> Tokenize(string tagStr)
		{
			var position = 0;

			while (position < tagStr.Length)
			{
				var (token, size) = NextToken(tagStr, position);
				if (token.Type != TagToken.Space)
				{
					yield return token;
				}

				position += size;
			}
		}

		public static (Token, int) NextToken(string tagStr, int position)
		{
			var charToken = tagStr[position];

			if (charToken == ':')
			{
				if (tagStr.Substring(position, 2) == "::")
				{
					return (new Token(TagToken.Colon, "::"), 2);
				}

				throw new Exception("Unrecognized token.");
			}
			if (charToken == '<')
			{
				return (new Token(TagToken.LT, "<"), 1);
			}
			if (charToken == '>')
			{
				return (new Token(TagToken.GT, ">"), 1);
			}
			if (charToken == ',')
			{
				return (new Token(TagToken.Comma, ","), 1);
			}
			if (charToken == ' ')
			{
				var res = "";
				for (int i = position; i < tagStr.Length; i++)
				{
					var curChar = tagStr[i];
					if (curChar == ' ')
					{
						res += curChar;
					}
					else
					{
						break;
					}
				}
				return (new Token(TagToken.Space, res), res.Length);
;			}
			if (char.IsLetter(charToken))
			{
				var res = "";
				for (int i = position; i < tagStr.Length; i++)
				{
					var curChar = tagStr[i];
					if (char.IsLetter(curChar))
					{
						res += curChar;
					}
					else
					{
						break;
					}
				}
				return (new Token(TagToken.Ident, res), res.Length);
			}
			
			throw new Exception("Unrecognized token.");
		}
		
		public static bool EnsureBoolean(object value)
		{
			if (value is bool)
			{
				return (bool)value;
			}

			if (value is string)
			{
				if ((string)value == "true")
				{
					return true;
				}
				if ((string)value == "false")
				{
					return false;
				}
			}

			throw GetException<bool>();
		}

		public static int EnsureInt(object value)
		{
			if (value is int)
			{
				return (int)value;
			}

			if (value is string)
			{
				return int.Parse((string)value);
			}

			throw GetException<int>();
		}

		public static ulong EnsureLong(object value)
		{
			if (value is ulong)
			{
				return (ulong)value;
			}
			
			if (value is string)
			{
				return ulong.Parse((string)value);
			}

			throw GetException<ulong>();
		}
		
		public static BigInteger EnsureBigInt(object value)
		{
			if (value is BigInteger)
			{
				return (BigInteger)value;
			}
			
			if (value is string)
			{
				return BigInteger.Parse((string)value);
			}

			throw GetException<BigInteger>();
		}
		
		public static byte[] EnsureAddress(object value)
		{
			if (value is string)
			{
				return ((string)value).HexToByteArray();
			}
			else if (value is byte[])
			{
				return (byte[])value;
			}
			else
			{
				throw new Exception("Invalid account address.");
			}
		}

		public static void SerializeArgs(object argValue, TypeTag argType, Serializer serializer)
		{
			if (argType is TypeTagBool)
			{
				serializer.SerializeBool(EnsureBoolean(argValue));
				return;
			}
			if (argType is TypeTagU8)
			{
				serializer.SerializeUint8((uint)EnsureInt(argValue));
				return;
			}
			if (argType is TypeTagU64)
			{
				serializer.SerializeUint64((uint)EnsureLong(argValue));
				return;
			}
			if (argType is TypeTagU128)
			{
				serializer.SerializeUint128(EnsureBigInt(argValue));
				return;
			}
			if (argType is TypeTagAddress)
			{
				serializer.SerializeBytes(EnsureAddress(argValue));
				return;
			}
			if (argType is TypeTagVector)
			{
				var type = ((TypeTagVector)argType).Value;
				if (type is TypeTagU8)
				{
					if (argValue is byte[])
					{
						serializer.SerializeBytes((byte[])argValue);						
					}

					if (argValue is string)
					{
						serializer.SerializeString((string)argValue);
					}
				}

				if (!(argValue is Array))
				{
					throw new Exception("Invalid vector args.");
				}
				
				var vector = (Array)argValue;
				
				serializer.SerializeUInt32AsUleb128((uint)vector.Length);
				foreach (var item in vector)
				{
					SerializeArgs(item, type, serializer);
				}

				return;
			}
			if (argType is TypeTagStruct)
			{
				if (!(argValue is string))
				{
					throw GetException<string>();
				}
				
				var structValue = ((TypeTagStruct)argValue).Value;
				if ($"{structValue.Address.ToHexCompact(true)}::{structValue.ModuleName}::{structValue.Name}" != "0x1::string::String")
				{
					throw new Exception("The only supported struct arg is of type 0x1::string::String");
				}
				
				serializer.SerializeString((string)argValue);
				
				return;
			}
			
			throw new Exception("Unsupported arg type.");
		}

		public static TransactionArgument ArgToTransactionArgument(object argValue, TypeTag argType)
		{
			if (argType is TypeTagBool)
			{
				return new TransactionArgumentBool(EnsureBoolean(argValue));
			}
			if (argType is TypeTagU8)
			{
				return new TransactionArgumentU8((uint)EnsureInt(argValue));
			}
			if (argType is TypeTagU64)
			{
				return new TransactionArgumentU64(EnsureLong(argValue));
			}
			if (argType is TypeTagU128)
			{
				return new TransactionArgumentU128(EnsureBigInt(argValue));
			}
			if (argType is TypeTagAddress)
			{
				return new TransactionArgumentAddress(EnsureAddress(argValue));
			}
			if (argType is TypeTagVector && ((TypeTagVector)argType).Value is TypeTagU8)
			{
				if (!(argValue is Array))
				{
					throw new Exception("Invalid vector args.");
				}
				return new TransactionArgumentU8Vector((byte[])argValue);
			}
			
			throw new Exception("Unknown type for TransactionArgument.");
		}

		private static Exception GetException<T>()
		{
			return new Exception($"Value is not a {nameof(T)}.");
		}
	}
}