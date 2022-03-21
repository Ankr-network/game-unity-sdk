using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Core.Data.ContractMessages.ERC1155
{
	[Function("balanceOf", "uint256")]
	public class BalanceOfMessage : FunctionMessage
	{
		[Parameter("address", "_account", 1)]
		public string Account { get; set; }
		
		[Parameter("uint256", "_id", 2)]
		public string Id { get; set; }
	}
}