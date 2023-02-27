using AnkrSDK.Plugins.WalletConnectSharp.Core;
using AnkrSDK.Plugins.WalletConnectSharp.Core.Events;
using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;
using AnkrSDK.Plugins.WalletConnectSharp.Core.Network;
using UnityEngine;

namespace AnkrSDK.Plugins.WalletConnectSharp.Unity
{
	public static class WalletConnectSessionFactory
	{
		public static WalletConnectSession RestoreWalletConnectSession(SavedSession savedSession, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null)
		{
			Debug.Log("RestoreWalletConnectSession");
			return new WalletConnectSession(savedSession, transport, cipher, eventDelegator);
		}

		public static WalletConnectSession GetNewWalletConnectSession(ClientMeta clientMeta,
			string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null)
		{
			Debug.Log("GetNewWalletConnectSession");
			return new WalletConnectSession(clientMeta, bridgeUrl, transport, cipher, chainId,
				eventDelegator);
		}
	}
}