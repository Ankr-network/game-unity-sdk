using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.Scripts.ContractMessages
{
	[Function("totalSupply", "uint256")]
	public class TotalSupplyMessage : FunctionMessage
	{
		
	}
}