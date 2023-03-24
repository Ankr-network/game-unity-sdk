using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Data.ContractMessages.ERC721
{
	[Function("getApproved", "address")]
	public class GetApprovedMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId")]
		public BigInteger TokenID { get; set; }
	}
}