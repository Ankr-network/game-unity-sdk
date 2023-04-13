using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("SafeMinted")]
	public class SafeMintedEventDTO : EventDTOBase
	{
		[Parameter("address", "to", 1, true)] 
		public string To { get; set; }
	}
}