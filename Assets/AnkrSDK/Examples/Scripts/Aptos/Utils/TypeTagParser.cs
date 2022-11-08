using System;
using System.Collections.Generic;
using System.Linq;
using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.Imlementation;

namespace AnkrSDK.Aptos.Utils
{
	public class TypeTagParser
	{
		private readonly List<Token> _tokens;

		public TypeTagParser(string tagStr)
		{
			_tokens = BuildUtils.Tokenize(tagStr).ToList();
		}

		public TypeTag ParseTypeTag()
		{
			if (_tokens.Count == 0)
			{
				throw GetInvalidTokenException();
			}
			
			var token = PopLeft();

			if (token.Value == "u8")
			{
				return new TypeTagU8();
			}
			if (token.Value == "u64")
			{
				return new TypeTagU64();
			}
			if (token.Value == "u128")
			{
				return new TypeTagU128();
			}
			if (token.Value == "bool")
			{
				return new TypeTagBool();
			}
			if (token.Value == "address")
			{
				return new TypeTagAddress();
			}
			if (token.Value == "vector")
			{
				Consume("<");
				var res = ParseTypeTag();
				Consume(">");
				return new TypeTagVector(res);
			}
			if (token.Type == TagToken.Ident && (token.Value.StartsWith("0x") || token.Value.StartsWith("0X")))
			{
				var address = token.Value;
				Consume("::");
				var moduleToken = PopLeft();
				if (moduleToken.Type != TagToken.Ident)
				{
					throw GetInvalidTokenException();
				}
				Consume("::");
				var nameToken = PopLeft();
				if (nameToken.Type != TagToken.Ident)
				{
					throw GetInvalidTokenException();
				}

				var typeTags = new List<TypeTag>();
				if (_tokens.Count > 0 && _tokens.First().Value == "<")
				{
					Consume("<");
					var argsTypes = ParseCommaList(">", true);
					Consume(">");
				}

				var structTag = new StructTag(address.HexToByteArray((int)TypeLength.Address), moduleToken.Value, nameToken.Value, typeTags.ToArray());

				return new TypeTagStruct(structTag);
			}

			throw GetInvalidTokenException();
		}

		private IEnumerable<TypeTag> ParseCommaList(string endToken, bool allowTraillingComma)
		{
			ThrowExceptionIfListIsEmpty();

			while (_tokens.First().Value != endToken)
			{
				yield return ParseTypeTag();

				if (_tokens.Count > 0 && _tokens.First().Value == endToken)
				{
					break;
				}
				
				Consume(",");
				
				if (_tokens.Count > 0 && _tokens.First().Value == endToken && allowTraillingComma)
				{
					break;
				}

				ThrowExceptionIfListIsEmpty();
			}
		}

		private void Consume(string targetToken)
		{
			var token = PopLeft();
			if (token == null || token.Value != targetToken)
			{
				throw new Exception("Invalid type tag.");
			}
		}

		private Token PopLeft()
		{
			if (_tokens.Count > 0)
			{
				var token = _tokens[0];
				_tokens.RemoveAt(0);
				return token;
			}
			else
			{
				return null;
			}
		}
		
		private void ThrowExceptionIfListIsEmpty()
		{
			if (_tokens.Count <= 0)
			{
				throw GetInvalidTokenException();
			}
		}
		
		private Exception GetInvalidTokenException()
		{
			return new Exception("Invalid type tag.");
		}
	}
}