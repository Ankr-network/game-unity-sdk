using System;
using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnkrSDK.Utils
{
	public static class WalletConnectProvider
	{
		private static WalletConnect _walletConnect;

		public static WalletConnect GetWalletConnect()
		{
			if (_walletConnect != null)
			{
				return _walletConnect;
			}
			
			CreateWalletConnect();

			return _walletConnect;
		}

		private static void CreateWalletConnect()
		{
			var connectAdapter = Object.FindObjectOfType<WalletConnectUnityMonoAdapter>();

			if (connectAdapter == null)
			{
				throw new ArgumentNullException(nameof(_walletConnect),
					$"Couldn't find a valid {nameof(WalletConnectUnityMonoAdapter)} Instance on scene. Please make sure you have an instance ready to be used");
			}

			_walletConnect = new WalletConnect();
			var settings = Resources.Load<WalletConnectSettingsSO>("WalletConnectSettings");
			_walletConnect.Initialize(settings);
			connectAdapter.SetupWalletConnect(_walletConnect);
		}
	}
}