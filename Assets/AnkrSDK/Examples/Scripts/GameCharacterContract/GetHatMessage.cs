using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.GameCharacterContract
{
	[Function("getHat", "uint256")]
	public class GetHatMessage : FunctionMessage
	{
		[Parameter("uint256", "_characterId")]
		public BigInteger CharacterId { get; set; }
	}
}