using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.Scripts.ContractMessages
{
	[Function("symbol", "string")]
	public class SymbolMessage : FunctionMessage
	{
		
	}
}