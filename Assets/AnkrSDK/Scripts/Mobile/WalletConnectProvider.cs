using System;
using AnkrSDK.WalletConnectSharp.Unity;

namespace AnkrSDK.Mobile
{
	internal static class WalletConnectProvider
	{
		private static WalletConnect _walletConnect;

		internal static WalletConnect GetWalletConnect()
		{
			if (_walletConnect != null)
			{
				return _walletConnect;
			}

			_walletConnect = UnityEngine.Object.FindObjectOfType<WalletConnect>();

			if (_walletConnect == null)
			{
				throw new ArgumentNullException(nameof(_walletConnect),
					"Couldn't find a valid WalletConnect Instance on scene. Please make sure you have an instance ready to be used");
			}

			return _walletConnect;
		}
	}
}