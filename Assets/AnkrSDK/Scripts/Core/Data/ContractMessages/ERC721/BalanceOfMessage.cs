using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Core.Data.ContractMessages.ERC721
{
	[Function("balanceOf", "uint256")]
	public class BalanceOfMessage : FunctionMessage
	{
		[Parameter("address", "_owner", 1)]
		public string Owner { get; set; }
	}
}