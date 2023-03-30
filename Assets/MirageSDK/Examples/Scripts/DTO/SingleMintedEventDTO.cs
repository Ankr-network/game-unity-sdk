using System.Collections.Generic;
using System.Numerics;
using MirageSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.DTO
{
	[Event("SingleMinted")]
	public class SingleMintedEventDTO : EventDTOBase
	{
		[Parameter("address", "account", 1, true)]
		public string Account { get; set; }

		[Parameter("uint256", "id", 2, true)]
		public BigInteger Id { get; set; }

		[Parameter("uint256", "amount", 3, false)]
		public BigInteger Amount { get; set; }

		[Parameter("bytes", "data", 4, false)]
		public byte[] Data { get; set; }
	}
}