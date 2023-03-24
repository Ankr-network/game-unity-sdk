using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Data.ContractMessages.ERC721
{
	[Function("ownerOf", "address")]
	public class OwnerOfMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId", 1)]
		public BigInteger TokenId { get; set; }
		
	}
}