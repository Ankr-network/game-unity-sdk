using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Examples.ContractMessages.ERC721
{
	[Function("totalSupply", "uint256")]
	public class TotalSupplyMessage : FunctionMessage
	{
		
	}
}