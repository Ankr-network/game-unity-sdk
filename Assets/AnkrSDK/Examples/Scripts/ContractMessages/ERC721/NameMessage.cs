using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AnkrSDK.Examples.ContractMessages.ERC721
{
	[Function("name", "string")]
	public class NameMessage : FunctionMessage
	{
		
	}
}