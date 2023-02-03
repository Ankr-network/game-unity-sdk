namespace AnkrSDK.WalletConnectSharp.Core
{
	public static class WalletConnectStatusExtensions
	{
		public static bool IsAny(this WalletConnectStatus status, WalletConnectStatus statusFlags)
		{
			return (status & statusFlags) != 0;
		}
	}
}