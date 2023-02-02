using System;

namespace AnkrSDK.WalletConnectSharp.Core
{
	[Flags]
	public enum WalletConnectStatus
	{
		Uninitialized = 0,
		DisconnectedNoSession = 1,
		DisconnectedSessionCached = 2,
		TransportConnected = 4,
		SessionConnected = 8,
		WalletConnected = 16,
		SessionOrWalletConnected = SessionConnected | WalletConnected,
		AnythingConnected = TransportConnected | SessionConnected | WalletConnected
	}
}