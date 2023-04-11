using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Data.ContractMessages.ERC721
{
	[Function("tokenByIndex", "uint256")]
	public class TokenByIndexMessage : FunctionMessage
	{
		[Parameter("uint256", "_index", 1)]
		public BigInteger Index { get; set; }
	}
}