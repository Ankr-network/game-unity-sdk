using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.GameCharacterContract
{
	[Function("getShoes", "uint256")]
	public class GetShoesMessage : FunctionMessage
	{
		[Parameter("uint256", "_characterId")]
		public BigInteger CharacterId { get; set; }
	}
}