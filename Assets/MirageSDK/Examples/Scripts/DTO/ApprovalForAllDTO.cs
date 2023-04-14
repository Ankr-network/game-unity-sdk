using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("ApprovalForAll")]
	public class ApprovalForAllEventDTO : EventDTOBase
	{
		[Parameter("address", "account", 1, true)]
		public string Account { get; set; }

		[Parameter("address", "operator", 2, true)]
		public string Operator { get; set; }

		[Parameter("bool", "approved", 3, false)]
		public bool Approved { get; set; }
	}
}