using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Core.Data.ContractMessages.ERC721
{
	[Function("tokenURI", "string")]
	public class TokenURIMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId")]
		public string TokenId { get; set; }
	}
}