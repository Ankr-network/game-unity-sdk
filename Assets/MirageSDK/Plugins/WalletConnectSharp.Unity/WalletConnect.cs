using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using MirageSDK.WalletConnect.VersionShared;
using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnect.VersionShared.Models;
using MirageSDK.WalletConnect.VersionShared.Models.DeepLink;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum.Types;
using MirageSDK.WalletConnect.VersionShared.Utils;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Core.Models;
using MirageSDK.WalletConnectSharp.Core.Network;
using MirageSDK.WalletConnectSharp.Unity.Network;
using MirageSDK.WalletConnectSharp.Unity.Utils;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core.StatusEvents;
using UnityEngine;
using Logger = AnkrSDK.InternalUtils.Logger;

[assembly: InternalsVisibleTo("MirageSDK.Tests.Runtime")]

namespace MirageSDK.WalletConnectSharp.Unity
{
	public class WalletConnect : IWalletConnectable, IWalletConnectStatusHolder,
		IWalletConnectGenericRequester, IWalletConnectCommunicator,
		IQuittable, IPausable, IUpdatable
	{
		private const string SettingsFilenameString = "WalletConnectSettings";
		public event Action<WalletConnectTransitionBase> SessionStatusUpdated;
		public event Action OnSend;

		public event Action<string[]> OnAccountChanged;
		public event Action<int> OnChainChanged;
		public bool CanSendRequests => _session?.CanSendRequests ?? false;

		public WalletConnectStatus Status => _session?.Status ?? WalletConnectStatus.Uninitialized;

		public string PeerId
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.PeerId;
			}
		}

		public ClientMeta DappMetadata
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.DappMetadata;
			}
		}

		public ClientMeta WalletMetadata
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.WalletMetadata;
			}
		}

		public string WalletName
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.WalletMetadata?.Name;
			}
		}

		public string[] Accounts
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.Accounts;
			}
		}

		public int NetworkId
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.NetworkId ?? -1;
			}
		}

		public int ChainId
		{
			get
			{
				CheckIfSessionCreated();
				return _session?.ChainId ?? -1;
			}
		}

		public bool Connecting => _session != null && _session.Connecting;

		private WalletConnectStatus _previousStatus;
		private readonly NativeWebSocketTransport _transport = new NativeWebSocketTransport();
		private WalletConnectSettingsSO _settings;
		private bool _initialized;
		public string ConnectURL => _session.URI;
		public string SettingsFilename => SettingsFilenameString;

		private WalletEntry _selectedWallet;
		private WalletConnectSession _session;
		private VersionInfo _ownVersionKnowledge;

		public void Initialize(ScriptableObject settings)
		{
			_settings = settings as WalletConnectSettingsSO;
			if (_settings != null)
			{
				_initialized = true;
			}
			else
			{
				var typeStr = settings == null ? "null" : settings.GetType().Name;
				Debug.LogError("WalletConnect: Could not initialize because settings are " + typeStr);
			}
		}

		public void Update()
		{
			_transport?.Update();

			if (_session != null)
			{
				var newStatus = _session.Status;
				if (_previousStatus != newStatus)
				{
					var transition = TransitionDataFactory.CreateTransitionObj(_previousStatus, newStatus, _session);
					_previousStatus = newStatus;
					SessionStatusUpdated?.Invoke(transition);
				}
			}
		}

		public UniTask Quit()
		{
			return SaveOrDisconnect();
		}

		public void Dispose()
		{
			SaveOrDisconnect().Forget();
		}

		public async UniTask OnApplicationPause(bool pauseStatus)
		{
			if (!_initialized)
			{
				throw new InvalidOperationException("WalletConnect is not initialized");
			}

			if (_transport != null)
			{
				await _transport.OnApplicationPause(pauseStatus);
			}

			if (pauseStatus)
			{
				await SaveOrDisconnect();
			}
			else if (SessionSaveHandler.IsSessionSaved() && _settings.AutoSaveAndResume)
			{
				await Connect();
			}
		}

		public async UniTask<string> ReconnectSession()
		{
			SessionSaveHandler.ClearSession();
			_session = null;
			Connect().Forget();
			await UniTask.WaitUntil(() => _session != null && _session.Status == WalletConnectStatus.SessionRequestSent);
			return _session?.URI;
		}

		public async UniTask Connect()
		{
			//disabled for now because of CORS policy limitation
			//we need to change the CORS policy in
			//'https://usage-stats.game.ankr.com/collect
			//endpoint to make it work in WebGL
			#if !UNITY_WEBGL
			TryLoadOwnVersionKnowledge();
			LogVersion();
			#endif

			var savedSession = SessionSaveHandler.GetSavedSession();

			if (_session != null)
			{
				var status = _session.Status;
				if (savedSession != null)
				{
					if (_session.KeyData != savedSession.Key)
					{
						if (status == WalletConnectStatus.WalletConnected)
						{
							await _session.DisconnectSession();
						}
						else if (status == WalletConnectStatus.TransportConnected)
						{
							await _session.Transport.Close();
						}
					}
					else if (status != WalletConnectStatus.WalletConnected && !_session.Connecting)
					{
						await CompleteConnect();
						return;
					}
					else
					{
						Debug.Log("Nothing to do, we are already connected and session key did not change");
						return;
					}
				}
				else if (status == WalletConnectStatus.WalletConnected)
				{
					Debug.Log("We have old session connected, but no saved session. Disconnecting.");
					await _session.DisconnectSession();
				}
				else if (status == WalletConnectStatus.TransportConnected)
				{
					Debug.Log("We have transport connected, but no saved session. Closing Transport.");
					await _session.Transport.Close();
				}
				else if (_session.Connecting)
				{
					Debug.Log("Session connection is in progress. Connect request ignored.");
					return;
				}
			}

			InitializeSession(savedSession);

			await CompleteConnect();
		}

		public UniTask<string> EthSign(string address, string message)
		{
			CheckIfSessionCreated();
			return _session.EthSign(address, message);
		}

		public UniTask<string> EthPersonalSign(string address, string message)
		{
			CheckIfSessionCreated();
			return _session.EthPersonalSign(address, message);
		}

		public UniTask<string> EthSignTypedData<T>(string address, T data, EIP712Domain eip712Domain)
		{
			CheckIfSessionCreated();
			return _session.EthSignTypedData(address, data, eip712Domain);
		}

		public UniTask<string> EthSendTransaction(params TransactionData[] transaction)
		{
			CheckIfSessionCreated();
			return _session.EthSendTransaction(transaction);
		}

		public UniTask<string> EthSignTransaction(params TransactionData[] transaction)
		{
			CheckIfSessionCreated();
			return _session.EthSignTransaction(transaction);
		}

		public UniTask<string> EthSendRawTransaction(string data, Encoding messageEncoding = null)
		{
			CheckIfSessionCreated();
			return _session.EthSendRawTransaction(data, messageEncoding);
		}

		public UniTask<string> WalletAddEthChain(EthChainData chainData)
		{
			CheckIfSessionCreated();
			return _session.WalletAddEthChain(chainData);
		}

		public UniTask<string> WalletSwitchEthChain(EthChain chainData)
		{
			CheckIfSessionCreated();
			return _session.WalletSwitchEthChain(chainData);
		}

		public UniTask<string> WalletUpdateEthChain(EthUpdateChainData chainData)
		{
			CheckIfSessionCreated();
			return _session.WalletUpdateEthChain(chainData);
		}

		public UniTask<BigInteger> EthChainId()
		{
			CheckIfSessionCreated();
			return _session.EthChainId();
		}

		//network argument is not used because WC1
		//only supports Ethereum network but still kept here to
		//support unified interface with WC2
		public UniTask<string> GetDefaultAccount(string network = null)
		{
			CheckIfSessionCreated();
			return _session.GetDefaultAccount(network);
		}

		public UniTask<TResponse> Send<TRequest, TResponse>(TRequest request)
			where TRequest : IIdentifiable
			where TResponse : IErrorHolder
		{
			CheckIfSessionCreated();
			return _session.Send<TRequest, TResponse>(request);
		}

		public UniTask<GenericJsonRpcResponse> GenericRequest(GenericJsonRpcRequest genericRequest)
		{
			CheckIfSessionCreated();
			return _session.GenericRequest(genericRequest);
		}

		private void CheckIfSessionCreated()
		{
			if (_session == null)
			{
				Debug.LogError("Trying to access WalletConnect session before it was created");
			}
		}

		internal void InitializeSession(SavedSession savedSession = null, ICipher cipher = null)
		{
			if (!_initialized)
			{
				throw new InvalidOperationException("WalletConnect is not initialized");
			}

			var appData = _settings.AppData;
			var customBridgeUrl = _settings.CustomBridgeUrl;
			var chainId = _settings.ChainId;
			_session = savedSession != null
				? WalletConnectSessionFactory.RestoreWalletConnectSession(savedSession, _transport)
				: WalletConnectSessionFactory.GetNewWalletConnectSession(appData, customBridgeUrl, _transport,
					cipher, chainId);
		}

		public void OpenMobileWallet(WalletEntry selectedWallet)
		{
			_selectedWallet = selectedWallet;

			OpenMobileWallet();
		}

		public void OpenDeepLink(WalletEntry selectedWallet)
		{
			_selectedWallet = selectedWallet;

			OpenDeepLink();
		}

		public void OpenMobileWallet()
		{
			#if UNITY_ANDROID
			var signingURL = ConnectURL.Split('@')[0];

			Application.OpenURL(signingURL);
			#elif UNITY_IOS
			if (_selectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenMobileWallet(WalletEntry) or set _selectedWallet on iOS!");
			}

			var url = MobileWalletURLFormatHelper
				.GetURLForMobileWalletOpen(ConnectURL, _selectedWallet.mobile).Split('?')[0];

			Debug.Log("Opening: " + url);
			Application.OpenURL(url);
			#else
			Debug.Log("Platform does not support deep linking");
			return;
			#endif
		}

		public void OpenDeepLink()
		{
			if (_session.Status != WalletConnectStatus.SessionRequestSent)
			{
				Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
				               "\nWait for Status == WalletConnectStatus.ConnectionRequestSent");
				return;
			}

			#if UNITY_ANDROID
			Application.OpenURL(ConnectURL);
			#elif UNITY_IOS
			if (_selectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenDeepLink(WalletEntry) or set _selectedWallet on iOS!");
			}

			var url = MobileWalletURLFormatHelper
				.GetURLForMobileWalletOpen(ConnectURL, _selectedWallet.mobile);

			Debug.Log("[WalletConnect] Opening URL: " + url);

			Application.OpenURL(url);
			#else
			Debug.Log("Platform does not support deep linking");
			return;
			#endif
		}

		public async UniTask CloseSession(bool connectNewSession = true)
		{
			if (_session == null)
			{
				return;
			}

			await _session.DisconnectSession();

			if (connectNewSession)
			{
				await Connect();
			}
		}

		private void TryLoadOwnVersionKnowledge()
		{
			var ownVersionKnowledgeTextAsset = Resources.Load<TextAsset>("own-version-knowledge");
			_ownVersionKnowledge = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionInfo>(ownVersionKnowledgeTextAsset.text.Trim());
		}

		private void LogVersion()
		{
			if (_ownVersionKnowledge != null && !string.IsNullOrWhiteSpace(_ownVersionKnowledge.version))
			{
				Logger.AddLog(_ownVersionKnowledge.version);
				Debug.Log($"WalletConnect logged version {_ownVersionKnowledge.version} successfully");
			}
			else
			{
				Debug.LogError("Own version not found by WalletConnect");
			}
		}

		private void SetupEvents()
		{
			if (_session == null)
			{
				Debug.LogError("Trying to setup events on null session");
				return;
			}

			_session.OnSessionDisconnect += HandleSessionDisconnect;
			_session.OnSessionCreated += HandleSessionCreated;
			_session.OnSend += HandleOnSend;
			_session.OnAccountChanged += HandleOnAccountChanged;
			_session.OnChainChanged += HandleOnChainChanged;
		}

		private void TeardownEvents()
		{
			if (_session == null)
			{
				return;
			}

			_session.OnSessionDisconnect -= HandleSessionDisconnect;
			_session.OnSessionCreated -= HandleSessionCreated;
			_session.OnSend -= HandleOnSend;
			_session.OnAccountChanged -= HandleOnAccountChanged;
			_session.OnChainChanged -= HandleOnChainChanged;
		}

		private void HandleOnAccountChanged(string[] accounts)
		{
			OnAccountChanged?.Invoke(accounts);
		}

		private void HandleOnChainChanged(int chainId)
		{
			OnChainChanged?.Invoke(chainId);
		}

		private void HandleOnSend()
		{
			OnSend?.Invoke();
		}

		private void HandleSessionCreated()
		{
			var sessionToSave = _session.GetSavedSession();
			SessionSaveHandler.SaveSession(sessionToSave);
		}

		private async UniTask<WCSessionData> CompleteConnect()
		{
			SetupDefaultWallet().Forget();
			TeardownEvents();
			SetupEvents();

			var tries = 0;
			var connectSessionRetryCount = _settings.ConnectSessionRetryCount;
			while (tries < connectSessionRetryCount)
			{
				Debug.Log($"Trying to connect session. Try : {tries}");
				try
				{
					var sessionData = await _session.ConnectSession();

					return sessionData;
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

		private async void HandleSessionDisconnect()
		{
			if (_settings.AutoSaveAndResume && SessionSaveHandler.IsSessionSaved())
			{
				SessionSaveHandler.ClearSession();
			}

			if (_settings.CreateNewSessionOnSessionDisconnect)
			{
				await Connect();
			}
		}

		private async UniTask SetupDefaultWallet()
		{
			if (_settings.DefaultWallet == Wallets.None)
			{
				return;
			}

			var wallet = await WalletDownloadHelper.FindWalletEntry(_settings.DefaultWallet);

			if (wallet != null)
			{
				_selectedWallet = wallet;
				await wallet.DownloadImages();
			}
		}

		private UniTask SaveOrDisconnect()
		{
			if (_session == null)
			{
				return UniTask.CompletedTask;
			}

			if (!_session.Status.IsAny(WalletConnectStatus.WalletConnected))
			{
				return UniTask.CompletedTask;
			}

			if (!_settings.AutoSaveAndResume)
			{
				return _session.DisconnectSession();
			}

			var sessionToSave = _session.GetSavedSession();
			SessionSaveHandler.SaveSession(sessionToSave);

			return _session.Transport.Close();
		}
	}
}