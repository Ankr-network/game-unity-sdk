using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Events;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public class WalletConnectUnitySession : WalletConnectSession
	{
		internal string KeyData => Key;

		private WalletConnectUnitySession(SavedSession savedSession, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null) : base(savedSession, transport, cipher,
			eventDelegator)
		{
		}

		private WalletConnectUnitySession(ClientMeta clientMeta, string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) :
			base(clientMeta, bridgeUrl, transport, cipher, chainId, eventDelegator)
		{
		}

		public static WalletConnectUnitySession RestoreWalletConnectSession(SavedSession savedSession, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null)
		{
			Debug.Log("RestoreWalletConnectSession");
			return new WalletConnectUnitySession(savedSession, transport, cipher, eventDelegator);
		}

		public static WalletConnectUnitySession GetNewWalletConnectSession(ClientMeta clientMeta,
			string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null)
		{
			Debug.Log("GetNewWalletConnectSession");
			return new WalletConnectUnitySession(clientMeta, bridgeUrl, transport, cipher, chainId,
				eventDelegator);
		}

	}
}