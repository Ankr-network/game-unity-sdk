using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Data.ContractMessages.ERC721.RentableExtension
{
	[Function("isRented", "bool")]
	public class IsRentedMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId", 1)]
		public BigInteger TokenId { get; set; }
		
	}
}