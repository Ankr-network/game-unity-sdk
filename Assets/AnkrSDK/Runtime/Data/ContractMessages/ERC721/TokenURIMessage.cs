using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Data.ContractMessages.ERC721
{
	[Function("tokenURI", "string")]
	public class TokenURIMessage : FunctionMessage
	{
		[Parameter("uint256", "_tokenId")]
		public BigInteger TokenId { get; set; }
	}
}