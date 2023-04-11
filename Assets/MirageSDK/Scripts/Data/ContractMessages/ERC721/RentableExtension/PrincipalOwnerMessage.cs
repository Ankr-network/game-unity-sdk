using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Data.ContractMessages.ERC721.RentableExtension
{
	[Function("principalOwner", "address")]
	public class PrincipalOwnerMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId", 1)]
		public BigInteger TokenId { get; set; }
		
	}
}