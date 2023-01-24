using AnkrSDK.WalletConnectSharp.Unity;

namespace Tests.Runtime
{
	public static class WalletHelper
	{
		public static WalletConnect CreateWalletConnectObject()
		{
			var walletConnect = new WalletConnect();
			walletConnect.Initialize();
			return walletConnect;
		}
	}
}