using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Core.Data.ContractMessages.ERC721
{
	[Function("tokenOfOwnerByIndex", "uint256")]
	public class TokenOfOwnerByIndexMessage : FunctionMessage
	{
		[Parameter("address", "_owner", 1)]
		public string Owner { get; set; }
		
		[Parameter("uint256", "_index", 2)]
		public BigInteger Index { get; set; }
	}
}