﻿using AnkrSDK.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

[Event("SafeMinted")]
public class SafeMintedEventDTO : EventDTOBase
{
	[Parameter("address", "to", 1, true)]
	public string To { get; set; }
}