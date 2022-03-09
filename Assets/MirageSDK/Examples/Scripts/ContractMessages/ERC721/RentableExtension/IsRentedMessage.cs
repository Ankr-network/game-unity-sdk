using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.ContractMessages.ERC721.RentableExtension
{
	[Function("isRented", "bool")]
	public class IsRentedMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId", 1)]
		public string TokenId { get; set; }
		
	}
}