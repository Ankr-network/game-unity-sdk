using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.ContractMessages.ERC721
{
	[Function("name", "string")]
	public class NameMessage : FunctionMessage
	{
		
	}
}