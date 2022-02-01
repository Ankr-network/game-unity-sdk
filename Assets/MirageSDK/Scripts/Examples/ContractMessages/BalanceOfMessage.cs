using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.ContractMessages
{
	[Function("balanceOf", "uint256")]
	public class BalanceOfMessage : FunctionMessage
	{
		[Parameter("address", "_owner", 1)]
		public virtual string Owner { get; set; }
	}
}