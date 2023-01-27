using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;

namespace Tests.Runtime
{
	public static class WalletHelper
	{
		public static WalletConnect CreateWalletConnectObject()
		{
			var walletConnect = new WalletConnect();
			var settings = Resources.Load<WalletConnectSettingsSO>("WalletConnectSettings");
			walletConnect.Initialize(settings);
			return walletConnect;
		}
	}
}