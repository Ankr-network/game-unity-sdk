using AnkrSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AnkrSDK.DTO
{
	[Event("URISet")]
	public class URISetEventDTO : EventDTOBase
	{
		[Parameter("string", "uri", 1, true)]
		public string URI { get; set; }
	}
}