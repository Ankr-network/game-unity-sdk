using System;

namespace AnkrSDK.WalletConnect2
{
	[Flags]
	public enum WalletConnect2Status
	{
		Uninitialized = 0,
		Disconnected = 1,
		ConnectionRequestSent = 8, //flags are power of 2 to avoid collision and 2 and 4 reserved for additional intermediate states that might be needed later
		WalletConnected = 16,
		AnythingConnected = ConnectionRequestSent | WalletConnected
	}
}