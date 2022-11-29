using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.GameCharacterContract
{
	[Function("getShoes", "uint256")]
	public class GetShoesMessage : FunctionMessage
	{
		[Parameter("uint256", "_characterId")]
		public string CharacterId { get; set; }
	}
}