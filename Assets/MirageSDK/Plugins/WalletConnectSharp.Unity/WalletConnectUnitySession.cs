using System.Threading.Tasks;
using MirageSDK.Plugins.WalletConnectSharp.Core;
using MirageSDK.Plugins.WalletConnectSharp.Core.Events;
using MirageSDK.Plugins.WalletConnectSharp.Core.Models;
using MirageSDK.Plugins.WalletConnectSharp.Core.Network;
using MirageSDK.Plugins.WalletConnectSharp.Unity;

namespace WalletConnectSharp.Unity
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