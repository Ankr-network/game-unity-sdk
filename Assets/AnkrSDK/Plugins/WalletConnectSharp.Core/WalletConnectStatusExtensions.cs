namespace AnkrSDK.WalletConnectSharp.Core
{
	public static class WalletConnectStatusExtensions
	{
		public static bool IsAny(this WalletConnectStatus status, WalletConnectStatus statusFlags)
		{
			return (status & statusFlags) != 0;
		}

		public static bool IsEqualOrGreater(this WalletConnectStatus status, WalletConnectStatus otherStatus)
		{
			return status >= otherStatus;
		}
	}
}