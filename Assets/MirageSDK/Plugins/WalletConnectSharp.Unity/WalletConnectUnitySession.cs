using System.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Core.Events;
using MirageSDK.WalletConnectSharp.Core.Models;
using MirageSDK.WalletConnectSharp.Core.Network;

namespace MirageSDK.WalletConnectSharp.Unity
{
	public class WalletConnectUnitySession : WalletConnectSession
	{
		private WalletConnect unityObjectSource;

		private bool listenerAdded;

		private WalletConnectUnitySession(SavedSession savedSession, WalletConnect source, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null) : base(savedSession, transport, cipher,
			eventDelegator)
		{
			unityObjectSource = source;
		}

		private WalletConnectUnitySession(ClientMeta clientMeta, WalletConnect source, string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) :
			base(clientMeta, bridgeUrl, transport, cipher, chainId, eventDelegator)
		{
			unityObjectSource = source;
		}

		internal string KeyData => Key;

		internal async Task<WCSessionData> SourceConnectSession()
		{
			Connecting = true;
			var result = await base.ConnectSession();
			Connecting = false;
			return result;
		}

		public static WalletConnectUnitySession RestoreWalletConnectSession(SavedSession savedSession,
			WalletConnect source, ITransport transport = null,
			ICipher cipher = null, EventDelegator eventDelegator = null)
		{
			return new WalletConnectUnitySession(savedSession, source, transport, cipher, eventDelegator);
		}

		public static WalletConnectUnitySession GetNewWalletConnectSession(ClientMeta clientMeta, WalletConnect source, string bridgeUrl = null,
			ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null)
		{
			return new WalletConnectUnitySession(clientMeta, source, bridgeUrl, transport, cipher, chainId,
				eventDelegator);
		}

		public override async Task Connect()
		{
			await ConnectSession();
		}

		public override async Task<WCSessionData> ConnectSession()
		{
			return await unityObjectSource.Connect();
		}
	}
}