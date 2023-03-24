using System.Collections.Generic;
using System.Numerics;
using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("BatchMinted")]
	public class BatchMintedEventDTO : EventDTOBase
	{
		[Parameter("address", "to", 1, true)]
		public string To { get; set; }

		[Parameter("uint256[]", "ids", 2, false)]
		public List<BigInteger> Ids { get; set; }

		[Parameter("uint256[]", "amounts", 3, false)]
		public List<BigInteger> Amounts { get; set; }

		[Parameter("bytes", "data", 4, false)]
		public byte[] Data { get; set; }
	}
}