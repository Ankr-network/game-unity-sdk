using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Data.ContractMessages.ERC721
{
	[Function("isApprovedForAll", "bool")]
	public class IsApprovedForAllMessage : FunctionMessage
	{
		[Parameter("address", "_owner")]
		public string Owner { get; set; }

		[Parameter("address", "_operator", 2)]
		public string Operator { get; set; }
	}
}