using System.Numerics;
using AnkrSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AnkrSDK.DTO
{
	[Event("Transfer")]
	public class TransferEventDTO : EventDTOBase
	{
		[Parameter("address", "_from", 1, true)]
		public string From { get; set; }

		[Parameter("address", "_to", 2, true)]
		public string To { get; set; }

		[Parameter("uint256", "_value", 3, false)]
		public BigInteger Value { get; set; }
	}
}