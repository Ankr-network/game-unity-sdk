using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;
using WalletConnectSharp.Unity.Models;
using WalletConnectSharp.Unity.Network;
using WalletConnectSharp.Unity.Utils;

namespace WalletConnectSharp.Unity
{
	[RequireComponent(typeof(NativeWebSocketTransport))]
	public class WalletConnect : BindableMonoBehavior
	{
		private const string SessionKey = "__WALLETCONNECT_SESSION__";

		/// <summary>
		/// FOR FUTURE USE - when using W.C. for iOS this list will limit the wallets
		/// displayed to the user.
		/// </summary>
		public List<string> AllowedWalletIds;

		public Dictionary<string, AppEntry> SupportedWallets { get; private set; }

		public AppEntry SelectedWallet { get; set; }

		public Wallets DefaultWallet;

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

		public event EventHandler ConnectionStarted;

		[BindComponent] private NativeWebSocketTransport _transport;

		public static WalletConnect Instance { get; private set; }

		public static WalletConnectUnitySession ActiveSession => Instance.Session;

		public string ConnectURL => Session.URI;

		public bool autoSaveAndResume = true;
		public bool connectOnAwake = false;
		public bool connectOnStart = true;
		public bool createNewSessionOnSessionDisconnect = true;
		public int connectSessionRetryCount = 3;
		public string customBridgeUrl;

		public int chainId = 1;

		public WalletConnectEventNoSession ConnectedEvent;

		public WalletConnectEventWithSessionData ConnectedEventSession;

		public WalletConnectEventWithSession DisconnectedEvent;

		public WalletConnectEventWithSession ConnectionFailedEvent;
		public WalletConnectEventWithSession NewSessionConnected;
		public WalletConnectEventWithSession ResumedSessionConnected;

		public WalletConnectUnitySession Session { get; private set; }

		[SerializeField] public ClientMeta AppData;

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

			if (connectOnAwake)
			{
				await Connect();
			}
		}

		private async void Start()
		{
			if (connectOnStart && !connectOnAwake)
			{
				await Connect();
			}
		}

		public async Task<WCSessionData> Connect(CancellationToken cancellationToken = default)
		{
			var savedSession = GetSavedSession();

			if (string.IsNullOrWhiteSpace(customBridgeUrl))
			{
				customBridgeUrl = null;
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
						SetupDefaultWallet().Forget();

						SetupEvents();

						return await CompleteConnect(cancellationToken);
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

			SetupDefaultWallet().Forget();

			SetupEvents();

			return await CompleteConnect(cancellationToken);
		}

		public static SavedSession GetSavedSession()
		{
			if (IsSessionSaved())
			{
				var json = PlayerPrefs.GetString(SessionKey);
				return JsonConvert.DeserializeObject<SavedSession>(json);
			}

			return null;
		}

		public void InitializeUnitySession(SavedSession savedSession = null, ICipher cipher = null)
		{
			Session = savedSession != null
				? new WalletConnectUnitySession(savedSession, this, _transport)
				: new WalletConnectUnitySession(AppData, this, customBridgeUrl, _transport, cipher, chainId);
		}

		private void SetupEvents()
		{
		#if UNITY_EDITOR || DEBUG
			//Useful for debug logging
			Session.OnSessionConnect += (sender, session) => { Debug.Log("[WalletConnect] Session Connected"); };
		#endif

			Session.OnSessionDisconnect += SessionOnOnSessionDisconnect;
			Session.OnSessionCreated += SessionOnOnSessionCreated;
			Session.OnSessionResumed += SessionOnOnSessionResumed;

		#if UNITY_ANDROID || UNITY_IOS
			//Whenever we send a request to the Wallet, we want to open the Wallet app
			Session.OnSend += (sender, session) => OpenMobileWallet();
		#endif
		}

		private void TeardownEvents()
		{
			Session.OnSessionDisconnect -= SessionOnOnSessionDisconnect;
			Session.OnSessionCreated -= SessionOnOnSessionCreated;
			Session.OnSessionResumed -= SessionOnOnSessionResumed;
		}

		private void SessionOnOnSessionResumed(object sender, WalletConnectSession e)
		{
			ResumedSessionConnected?.Invoke(e as WalletConnectUnitySession ?? Session);
		}

		private void SessionOnOnSessionCreated(object sender, WalletConnectSession e)
		{
			NewSessionConnected?.Invoke(e as WalletConnectUnitySession ?? Session);
		}

		private async Task<WCSessionData> CompleteConnect(CancellationToken cancellationToken)
		{
			Debug.Log("Waiting for Wallet connection");

			ConnectionStarted?.Invoke(this, EventArgs.Empty);

			var allEvents = new WalletConnectEventWithSessionData();

			allEvents.AddListener(delegate(WCSessionData arg0)
			{
				ConnectedEvent.Invoke();
				ConnectedEventSession.Invoke(arg0);
			});

			var tries = 0;
			while (tries < connectSessionRetryCount)
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

					if (tries >= connectSessionRetryCount)
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

			if (autoSaveAndResume && IsSessionSaved())
			{
				ClearSession();
			}

			TeardownEvents();

			if (createNewSessionOnSessionDisconnect)
			{
				await Connect();
			}
		}

		public static bool IsSessionSaved()
		{
			return PlayerPrefs.HasKey(SessionKey);
		}

		private async UniTask SetupDefaultWallet()
		{
			await FetchWalletList(false);

			var wallet =
				SupportedWallets.Values.FirstOrDefault(a =>
					string.Equals(a.name, DefaultWallet.ToString(), StringComparison.CurrentCultureIgnoreCase));

			if (wallet != null)
			{
				await DownloadImagesFor(wallet.id);
				SelectedWallet = wallet;
				Debug.Log("Setup default wallet " + wallet.name);
			}
		}

		private IEnumerator DownloadImagesFor(string id, string[] sizes = null)
		{
			sizes ??= new[] { "sm", "md", "lg" };

			var data = SupportedWallets[id];

			foreach (var size in sizes)
			{
				var url = "https://registry.walletconnect.org/logo/" + size + "/" + id + ".jpeg";

				using var imageRequest = UnityWebRequestTexture.GetTexture(url);
				yield return imageRequest.SendWebRequest();

				if (imageRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
				}
				else
				{
					var texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
					var sprite = Sprite.Create(texture,
						new Rect(0.0f, 0.0f, texture.width, texture.height),
						new Vector2(0.5f, 0.5f), 100.0f);

					switch (size)
					{
						case "sm":
							data.smallIcon = sprite;
							break;
						case "md":
							data.medimumIcon = sprite;
							break;
						case "lg":
							data.largeIcon = sprite;
							break;
					}
				}
			}
		}

		//Todo return supported wallets here
		public async UniTask FetchWalletList(bool downloadImages = true)
		{
			using var webRequest =
				UnityWebRequest.Get("https://registry.walletconnect.org/data/wallets.json");
			// Request and wait for the desired page.
			await webRequest.SendWebRequest();

			if (webRequest.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log("Error Getting Wallet Info: " + webRequest.error);
			}
			else
			{
				var json = webRequest.downloadHandler.text;

				SupportedWallets = JsonConvert.DeserializeObject<Dictionary<string, AppEntry>>(json);

				if (downloadImages)
				{
					foreach (var id in SupportedWallets.Keys)
					{
						await DownloadImagesFor(id);
					}
				}
			}
		}

		private async void OnDestroy()
		{
			await SaveOrDisconnect();
		}

		private async void OnApplicationQuit()
		{
			await SaveOrDisconnect();
		}

		private async void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				await SaveOrDisconnect();
			}
			else if (IsSessionSaved() && autoSaveAndResume)
			{
				await Connect();
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

			if (autoSaveAndResume)
			{
				var sessionToSave = Session.GetSavedSession();
				SaveSession(sessionToSave);

				await Session.Transport.Close();
			}
			else
			{
				await Session.Disconnect();
			}
		}

		public static void SaveSession(SavedSession sessionToSave)
		{
			var json = JsonConvert.SerializeObject(sessionToSave);
			PlayerPrefs.SetString(SessionKey, json);
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

		public static void ClearSession()
		{
			PlayerPrefs.DeleteKey(SessionKey);
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
	}
}