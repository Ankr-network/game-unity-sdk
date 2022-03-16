using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Examples.ContractMessages.ERC721
{
	[Function("symbol", "string")]
	public class SymbolMessage : FunctionMessage
	{
		
	}
}