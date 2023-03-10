using AnkrSDK.WalletConnectSharp.Core;

namespace AnkrSDK.WalletConnect2.Data
{
	public static class WalletConnect2StatusExtensions
	{
		public static bool IsAny(this WalletConnect2Status status, WalletConnect2Status statusFlags)
		{
			return (status & statusFlags) != 0;
		}

		public static bool IsEqualOrGreater(this WalletConnect2Status status, WalletConnect2Status otherStatus)
		{
			return status >= otherStatus;
		}
	}
}