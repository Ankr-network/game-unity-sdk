using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Examples.ContractMessages.GameCharacterContract
{
	[Function("getHat", "uint256")]
	public class GetHatMessage : FunctionMessage
	{
		[Parameter("uint256", "_characterId")]
		public string CharacterId { get; set; }
	}
}