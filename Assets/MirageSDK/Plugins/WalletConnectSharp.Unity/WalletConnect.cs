using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Core.Models;
using MirageSDK.WalletConnectSharp.Core.Network;
using MirageSDK.WalletConnectSharp.Unity.Models.DeepLink;
using MirageSDK.WalletConnectSharp.Unity.Network;
using MirageSDK.WalletConnectSharp.Unity.Utils;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_IOS
using System.Net;
#endif

namespace MirageSDK.WalletConnectSharp.Unity
{
	[RequireComponent(typeof(NativeWebSocketTransport))]
	public class WalletConnect : BindableMonoBehavior
	{
		public static WalletConnect Instance { get; private set; }

		public static WalletConnectUnitySession ActiveSession => Instance.Session;

		[Serializable]
		public class WalletConnectEventNoSession : UnityEvent
		{
		}

		[Serializable]
		public class WalletConnectEventWithSession : UnityEvent<WalletConnectUnitySession>
		{
		}

		[Serializable]
		public class WalletConnectEventWithSessionData : UnityEvent<WCSessionData>
		{
		}

		[SerializeField] private Wallets _defaultWallet = Wallets.MetaMask;
		[SerializeField] private bool _autoSaveAndResume = true;
		[SerializeField] private bool _connectOnAwake;
		[SerializeField] private bool _connectOnStart = true;
		[SerializeField] private bool _createNewSessionOnSessionDisconnect = true;
		[SerializeField] private int _connectSessionRetryCount = 3;
		[SerializeField] private string _customBridgeUrl = "https://testbridge.yartu.io/";
		[SerializeField] private int _chainId = 1;
		[SerializeField] private ClientMeta _appData;

		[SerializeField] private WalletConnectEventNoSession _connectedEvent;
		[SerializeField] private WalletConnectEventWithSessionData ConnectedEventSession;
		[SerializeField] private WalletConnectEventWithSession DisconnectedEvent;
		[SerializeField] private WalletConnectEventWithSession ConnectionFailedEvent;
		[SerializeField] private WalletConnectEventWithSession NewSessionConnected;
		[SerializeField] private WalletConnectEventWithSession ResumedSessionConnected;

		[BindComponent] private NativeWebSocketTransport _transport;

		private AppEntry SelectedWallet { get; set; }

		private string ConnectURL => Session.URI;

		public WalletConnectUnitySession Session { get; private set; }

		public event EventHandler ConnectionStarted;

		public bool CreateNewSessionOnSessionDisconnect => _createNewSessionOnSessionDisconnect;

		public bool ConnectOnStart
		{
			set => _connectOnStart = value;
		}

		public bool ConnectOnAwake
		{
			set => _connectOnAwake = value;
		}

		public ClientMeta AppData
		{
			set => _appData = value;
		}

		public WalletConnectEventNoSession ConnectedEvent
		{
			get => _connectedEvent;
			set => _connectedEvent = value;
		}

		protected override async void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);

			Instance = this;

			base.Awake();

			if (_connectOnAwake)
			{
				await Connect();
			}
		}

		private async void Start()
		{
			if (_connectOnStart && !_connectOnAwake)
			{
				await Connect();
			}
		}

		private async void OnDestroy()
		{
			await SaveOrDisconnect();
		}

		private async void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				await SaveOrDisconnect();
			}
			else if (SessionSaveHandler.IsSessionSaved() && _autoSaveAndResume)
			{
				await Connect();
			}
		}

		private async void OnApplicationQuit()
		{
			await SaveOrDisconnect();
		}

		public async Task<WCSessionData> Connect()
		{
			TeardownEvents();
			var savedSession = SessionSaveHandler.GetSavedSession();

			if (string.IsNullOrWhiteSpace(_customBridgeUrl))
			{
				_customBridgeUrl = null;
			}

			if (Session != null)
			{
				var currentKey = Session.KeyData;
				if (savedSession != null)
				{
					if (currentKey != savedSession.Key)
					{
						if (Session.Connected)
						{
							await Session.Disconnect();
						}
						else if (Session.TransportConnected)
						{
							await Session.Transport.Close();
						}
					}
					else if (!Session.Connected && !Session.Connecting)
					{
						return await CompleteConnect();
					}
					else
					{
						return null; //Nothing to do
					}
				}
				else if (Session.Connected)
				{
					await Session.Disconnect();
				}
				else if (Session.TransportConnected)
				{
					await Session.Transport.Close();
				}
				else if (Session.Connecting)
				{
					//We are still connecting, do nothing
					return null;
				}
			}

		#if UNITY_WEBGL
            var cipher = new WebGlAESCipher();
			InitializeUnitySession(savedSession, cipher);
		#else
			InitializeUnitySession(savedSession);
		#endif

			return await CompleteConnect();
		}

		public void InitializeUnitySession(SavedSession savedSession = null, ICipher cipher = null)
		{
			Session = savedSession != null
				? WalletConnectUnitySession.RestoreWalletConnectSession(savedSession, this, _transport)
				: WalletConnectUnitySession.GetNewWalletConnectSession(_appData, this, _customBridgeUrl, _transport,
					cipher, _chainId);
		}

		public void OpenMobileWallet(AppEntry selectedWallet)
		{
			SelectedWallet = selectedWallet;

			OpenMobileWallet();
		}

		public void OpenDeepLink(AppEntry selectedWallet)
		{
			SelectedWallet = selectedWallet;

			OpenDeepLink();
		}

		public void OpenMobileWallet()
		{
		#if UNITY_ANDROID
			var signingURL = ConnectURL.Split('@')[0];

			Application.OpenURL(signingURL);
		#elif UNITY_IOS
			if (SelectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenMobileWallet(AppEntry) or set SelectedWallet on iOS!");
			}
			else
			{
				string url;
				var encodedConnect = WebUtility.UrlEncode(ConnectURL);
				if (!string.IsNullOrWhiteSpace(SelectedWallet.mobile.universal))
				{
					url = SelectedWallet.mobile.universal + "/wc?uri=" + encodedConnect;
				}
				else
				{
					url = SelectedWallet.mobile.native + (SelectedWallet.mobile.native.EndsWith(":") ? "//" : "/") +
					      "wc?uri=" + encodedConnect;
				}

				var signingUrl = url.Split('?')[0];

				Debug.Log("Opening: " + signingUrl);
				Application.OpenURL(signingUrl);
			}
		#else
			Debug.Log("Platform does not support deep linking");
			return;
		#endif
		}

		public void OpenDeepLink()
		{
			if (!ActiveSession.ReadyForUserPrompt)
			{
				Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
				               "\nWait for ActiveSession.ReadyForUserPrompt to be true");

				return;
			}

		#if UNITY_ANDROID
			Debug.Log("[WalletConnect] Opening URL: " + ConnectURL);
			Application.OpenURL(ConnectURL);
		#elif UNITY_IOS
			if (SelectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenDeepLink(AppEntry) or set SelectedWallet on iOS!");
			}
			else
			{
				string url;
				string encodedConnect = WebUtility.UrlEncode(ConnectURL);
				if (!string.IsNullOrWhiteSpace(SelectedWallet.mobile.universal))
				{
					url = SelectedWallet.mobile.universal + "/wc?uri=" + encodedConnect;
				}
				else
				{
					url = SelectedWallet.mobile.native + (SelectedWallet.mobile.native.EndsWith(":") ? "//" : "/") +
					      "wc?uri=" + encodedConnect;
				}

				Debug.Log("Opening: " + url);
				Application.OpenURL(url);
			}
		#else
			Debug.Log("Platform does not support deep linking");
			return;
		#endif
		}

		public async void CloseSession(bool waitForNewSession = true)
		{
			if (ActiveSession == null)
			{
				return;
			}

			await ActiveSession.Disconnect();

			if (waitForNewSession)
			{
				await ActiveSession.Connect();
			}
		}

		private void SetupEvents()
		{
			if (Session == null)
			{
				Debug.LogError("Trying to setup events on null session");
				return;
			}
			
			Session.OnSessionDisconnect += SessionOnOnSessionDisconnect;
			Session.OnSessionCreated += SessionOnOnSessionCreated;
			Session.OnSessionResumed += SessionOnOnSessionResumed;

		#if UNITY_ANDROID || UNITY_IOS
			Session.OnSend += SessionOnSendEvent;
		#endif
		}

		private void TeardownEvents()
		{
			if (Session == null)
			{
				return;
			}

			Session.OnSessionDisconnect -= SessionOnOnSessionDisconnect;
			Session.OnSessionCreated -= SessionOnOnSessionCreated;
			Session.OnSessionResumed -= SessionOnOnSessionResumed;
		#if UNITY_ANDROID || UNITY_IOS
			Session.OnSend -= SessionOnSendEvent;
		#endif
		}

		private void SessionOnSendEvent(object sender, WalletConnectSession session)
		{
			OpenMobileWallet();
		}

		private void SessionOnOnSessionResumed(object sender, WalletConnectSession e)
		{
			ResumedSessionConnected?.Invoke(e as WalletConnectUnitySession ?? Session);
		}

		private void SessionOnOnSessionCreated(object sender, WalletConnectSession e)
		{
			NewSessionConnected?.Invoke(e as WalletConnectUnitySession ?? Session);

			var sessionToSave = Session.GetSavedSession();
			SessionSaveHandler.SaveSession(sessionToSave);
		}

		private async Task<WCSessionData> CompleteConnect()
		{
			SetupDefaultWallet().Forget();
			SetupEvents();

			ConnectionStarted?.Invoke(this, EventArgs.Empty);

			var allEvents = new WalletConnectEventWithSessionData();

			allEvents.AddListener(sessionData =>
			{
				_connectedEvent.Invoke();
				ConnectedEventSession.Invoke(sessionData);
			});

			var tries = 0;
			while (tries < _connectSessionRetryCount)
			{
				try
				{
					var session = await Session.SourceConnectSession();

					allEvents.Invoke(session);

					return session;
				}
				catch (IOException e)
				{
					tries++;

					if (tries >= _connectSessionRetryCount)
					{
						throw new IOException("Failed to request session connection after " + tries + " times.", e);
					}
				}
			}

			throw new IOException("Failed to request session connection after " + tries + " times.");
		}

		private async void SessionOnOnSessionDisconnect(object sender, EventArgs e)
		{
			DisconnectedEvent?.Invoke(ActiveSession);

			if (_autoSaveAndResume && SessionSaveHandler.IsSessionSaved())
			{
				SessionSaveHandler.ClearSession();
			}

			if (_createNewSessionOnSessionDisconnect)
			{
				await Connect();
			}
		}

		private async UniTask SetupDefaultWallet()
		{
			var supportedWallets = await WalletDownloadHelper.FetchWalletList(false);

			var wallet =
				supportedWallets.Values.FirstOrDefault(a =>
					string.Equals(a.name, _defaultWallet.ToString(), StringComparison.InvariantCultureIgnoreCase));

			if (wallet != null)
			{
				await wallet.DownloadImages();
				SelectedWallet = wallet;
				Debug.Log("Setup default wallet " + wallet.name);
			}
		}

		private async Task SaveOrDisconnect()
		{
			if (Session == null)
			{
				return;
			}

			if (!Session.Connected)
			{
				return;
			}

			if (_autoSaveAndResume)
			{
				var sessionToSave = Session.GetSavedSession();
				SessionSaveHandler.SaveSession(sessionToSave);

				await Session.Transport.Close();
			}
			else
			{
				await Session.Disconnect();
			}
		}
	}
}