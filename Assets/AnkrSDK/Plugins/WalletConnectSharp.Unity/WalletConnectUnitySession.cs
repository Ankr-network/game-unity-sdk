using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Events;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using UnityEngine;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public class WalletConnectUnitySession : WalletConnectSession
	{
		private readonly WalletConnect _unityObjectSource;

		internal string KeyData => Key;

		private WalletConnectUnitySession(SavedSession savedSession, WalletConnect source, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null) : base(savedSession, transport, cipher,
			eventDelegator)
		{
			_unityObjectSource = source;
		}

		private WalletConnectUnitySession(ClientMeta clientMeta, WalletConnect source, string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) :
			base(clientMeta, bridgeUrl, transport, cipher, chainId, eventDelegator)
		{
			_unityObjectSource = source;
		}

		public static WalletConnectUnitySession RestoreWalletConnectSession(SavedSession savedSession,
			WalletConnect source, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null)
		{
			Debug.Log("RestoreWalletConnectSession");
			return new WalletConnectUnitySession(savedSession, source, transport, cipher, eventDelegator);
		}

		public static WalletConnectUnitySession GetNewWalletConnectSession(ClientMeta clientMeta, WalletConnect source,
			string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null)
		{
			Debug.Log("GetNewWalletConnectSession");
			return new WalletConnectUnitySession(clientMeta, source, bridgeUrl, transport, cipher, chainId,
				eventDelegator);
		}

		public override Task Connect()
		{
			return ConnectSession();
		}

		public override Task<WCSessionData> ConnectSession()
		{
			return _unityObjectSource.Connect();
		}

		internal async Task<WCSessionData> WaitForSessionToConnectAsync()
		{
			Connecting = true;
			var result = await base.ConnectSession();
			Connecting = false;
			return result;
		}
	}
}