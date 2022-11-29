using MirageSDK.WalletConnectSharp.Core.Models;
using MirageSDK.WalletConnectSharp.Unity;
using MirageSDK.WalletConnectSharp.Unity.Network;
using UnityEngine;

namespace Tests.Runtime
{
	public static class WalletHelper
	{
		public static WalletConnect CreateWalletConnectObject()
		{
			var walletConnectObject = new GameObject("WalletConnect");
			var walletConnect = walletConnectObject.AddComponent<WalletConnect>();
			var transport = walletConnectObject.AddComponent<NativeWebSocketTransport>();
			walletConnect.Transport = transport;
			walletConnect.ConnectOnStart = false;
			walletConnect.ConnectOnAwake = false;
			walletConnect.AppData = new ClientMeta
			{
				_description = "Wallet",
				_url = "https://www.Mirage.com/",
				_icons = new[] { "https://www.Mirage.com/static/favicon/apple-touch-icon.png" },
				_name = "Wallet"
			};

			return walletConnect;
		}
	}
}