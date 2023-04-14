using System.Numerics;
using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("ShoesChanged")]
	public class ShoesChangedEventDTO : EventDTOBase
	{
		[Parameter("uint256", "characterId", 1, true)]
		public BigInteger CharacterId { get; set; }

		[Parameter("uint256", "oldShoesId", 2, false)]
		public BigInteger OldShoesId { get; set; }

		[Parameter("uint256", "newShoesId", 3, false)]
		public BigInteger NewShoesId { get; set; }
	}
}