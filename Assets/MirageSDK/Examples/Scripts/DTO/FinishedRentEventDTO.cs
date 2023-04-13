using System.Numerics;
using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("FinishedRent")]
	public class FinishedRentEventDTO : EventDTOBase
	{
		[Parameter("uint256", "tokenId", 1, true)]
		public BigInteger TokenId { get; set; }

		[Parameter("address", "lord", 2, true)]
		public string Lord { get; set; }

		[Parameter("address", "renter", 3, true)]
		public string Renter { get; set; }

		[Parameter("uint256", "expiresAt", 4, false)]
		public BigInteger ExpiresAt { get; set; }
	}
}