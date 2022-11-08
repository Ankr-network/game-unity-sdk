using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.Imlementation;
using AnkrSDK.Aptos.Imlementation.ABI;
using UnityEngine;

namespace AnkrSDK.Aptos.Utils
{
	public static class BuildUtils
	{
		private const string WhiteSpacesRegex = @"[\s]+";
		private const string AlphabeticRegex = @"[_A-Za-z0-9]+";

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
				var tokenPart = tagStr.Substring(position);
				var spaces = GetWhiteSpaces(tokenPart);

				return (new Token(TagToken.Space, tokenPart), tokenPart.Length);
			}

			if (char.IsLetter(charToken) || char.IsDigit(charToken))
			{
				var tokenPart = tagStr.Substring(position);
				var alphabeticSequence = GetAlphabeticSequence(tokenPart);

				return (new Token(TagToken.Ident, alphabeticSequence), alphabeticSequence.Length);
			}

			throw new Exception("Unrecognized token.");
		}

		public static string GetWhiteSpaces(string str)
		{
			return GetFirstRegexEntry(str, WhiteSpacesRegex);
		}

		public static string GetAlphabeticSequence(string str)
		{
			return GetFirstRegexEntry(str, AlphabeticRegex);
		}

		private static string GetFirstRegexEntry(string str, string regex)
		{
			Regex rx = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			var matches = rx.Matches(str);

			return matches.First().Groups.First().Value;
		}

		public static bool EnsureBoolean(object value)
		{
			if (value is bool boolValue)
			{
				return boolValue;
			}

			if (value is string stringValue)
			{
				if (stringValue == "true")
				{
					return true;
				}

				if (stringValue == "false")
				{
					return false;
				}
			}

			throw GetException<bool>();
		}

		public static uint EnsureInt(object value)
		{
			if (value is uint or ushort)
			{
				return (uint)value;
			}

			if (value is short shortValue)
			{
				return (ushort)shortValue;
			}

			if (value is string stringValue)
			{
				return uint.Parse(stringValue);
			}

			throw GetException<uint>();
		}

		public static ulong EnsureLong(object value)
		{
			if (value is ulong or uint or ushort or long)
			{
				return (ulong)value;
			}

			if (value is int intValue)
			{
				return (uint)intValue;
			}
			
			if (value is short shortValue)
			{
				return (ushort)shortValue;
			}

			if (value is string stringValue)
			{
				return ulong.Parse(stringValue);
			}

			throw GetException<ulong>();
		}

		public static BigInteger EnsureBigInt(object value)
		{
			if (value is BigInteger or ulong or uint or ushort or long or int or short or byte)
			{
				return (BigInteger)value;
			}

			if (value is string stringValue)
			{
				return BigInteger.Parse(stringValue);
			}

			throw GetException<BigInteger>();
		}

		public static byte[] EnsureAddress(object value)
		{
			if (value is string stringValue)
			{
				return stringValue.HexToByteArray((int)TypeLength.Address);
			}

			if (value is byte[] bytesValue)
			{
				return bytesValue;
			}

			throw new Exception("Invalid account address.");
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
				serializer.SerializeUint64(EnsureLong(argValue));
				return;
			}

			if (argType is TypeTagU128)
			{
				serializer.SerializeUint128(EnsureBigInt(argValue));
				return;
			}

			if (argType is TypeTagAddress)
			{
				serializer.SerializeFixedBytes(EnsureAddress(argValue));
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
				if ($"{structValue.Address.ToHexCompact(true)}::{structValue.ModuleName}::{structValue.Name}" !=
				    "0x1::string::String")
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
			return new Exception($"Value is not a {typeof(T)}.");
		}
	}
}