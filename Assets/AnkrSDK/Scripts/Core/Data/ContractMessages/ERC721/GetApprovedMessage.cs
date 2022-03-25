using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Core.Data.ContractMessages.ERC721
{
	[Function("getApproved", "address")]
	public class GetApprovedMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId")]
		public string TokenID { get; set; }
	}
}