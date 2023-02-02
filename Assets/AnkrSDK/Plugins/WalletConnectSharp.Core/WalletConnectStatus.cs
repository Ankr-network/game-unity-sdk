﻿using System;

namespace AnkrSDK.WalletConnectSharp.Core
{
	[Flags]
	public enum WalletConnectStatus
	{
		Uninitialized = 0,
		DisconnectedNoSession = 1,
		DisconnectedSessionCached = 2,
		TransportConnected = 4,
		SessionRequestSent = 8,
		SessionConnected = 16,
		AnythingConnected = TransportConnected | SessionRequestSent | SessionConnected
	}
}