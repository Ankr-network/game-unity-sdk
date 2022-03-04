using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.ContractMessages.ERC721
{
	[Function("symbol", "string")]
	public class SymbolMessage : FunctionMessage
	{
		
	}
}