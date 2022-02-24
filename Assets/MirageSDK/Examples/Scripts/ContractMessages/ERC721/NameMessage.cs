using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MirageSDK.Examples.Scripts.ContractMessages
{
	[Function("name", "string")]
	public class NameMessage : FunctionMessage
	{
		
	}
}