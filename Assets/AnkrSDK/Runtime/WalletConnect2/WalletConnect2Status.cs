using System;

namespace AnkrSDK.WalletConnect2
{
	[Flags]
	public enum WalletConnect2Status
	{
		Uninitialized = 0,
		Disconnected = 1,
		ConnectionRequestSent = 8,
		WalletConnected = 16
	}
}