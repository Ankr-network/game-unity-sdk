﻿using System.Numerics;
using AnkrSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

[Event("GlassesChanged")]
public class GlassesChangedEventDTO : EventDTOBase
{
	[Parameter("uint256", "characterId", 1, true)]
	public BigInteger CharacterId { get; set; }

	[Parameter("uint256", "oldGlassesId", 2, false)]
	public BigInteger OldGlassesId { get; set; }

	[Parameter("uint256", "newGlassesId", 3, false)]
	public BigInteger NewGlassesId { get; set; }
}