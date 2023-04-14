using System.Numerics;
using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("HatChanged")]
	public class HatChangedEventDTO : EventDTOBase
	{
		[Parameter("uint256", "characterId", 1, true)]
		public BigInteger CharacterId { get; set; }

		[Parameter("uint256", "oldHatId", 2, false)]
		public BigInteger OldHatId { get; set; }

		[Parameter("uint256", "newHatId", 3, false)]
		public BigInteger NewHatId { get; set; }
	}
}