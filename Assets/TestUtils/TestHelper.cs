using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity;
using AnkrSDK.WalletConnectSharp.Unity.Network;
using UnityEngine;

namespace TestUtils
{
	public static class TestHelper
	{
		public static SavedSession GetTestSession()
		{
			var clientMeta = new ClientMeta
			{
				_description = string.Empty,
				_icons = new[] { string.Empty },
				_name = string.Empty,
				_url = string.Empty
			};
			var testSession = new SavedSession(
				string.Empty,
				long.MinValue,
				string.Empty,
				string.Empty,
				new[] { byte.MinValue },
				string.Empty,
				0,
				new[] { string.Empty },
				0,
				clientMeta,
				clientMeta
			);
			return testSession;
		}

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
				_url = "https://www.ankr.com/",
				_icons = new[] { "https://www.ankr.com/static/favicon/apple-touch-icon.png" },
				_name = "Wallet"
			};

			return walletConnect;
		}
	}
}