using System;
using System.IO;
using System.Linq;
using AnkrSDK.Metadata;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink.Helpers;
using AnkrSDK.WalletConnectSharp.Unity.Network;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Logger = AnkrSDK.InternalUtils.Logger;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public partial class WalletConnect : IQuittable, IPausable, IUpdatable, IWalletConnectable
	{
		private const string SettingsFilenameString = "WalletConnectSettings";
		public WalletConnectStatus WalletConnectStatus => _session?.Status ?? WalletConnectStatus.Uninitialized;
		public WalletConnectProtocol Protocol
		{
			get
			{
				CheckIfSessionCreated();
				return _session;
			}
		}

		public string[] Accounts
		{
			get
			{
				CheckIfSessionCreated();
				return _session.Accounts;
			}
		}

		public int ChainId
		{
			get
			{
				CheckIfSessionCreated();
				return _session.ChainId;
			}
		}
		public bool Connecting => _session != null && _session.Connecting;

		private readonly NativeWebSocketTransport _transport = new NativeWebSocketTransport();
		
		private WalletConnectSettingsSO _settings;
		private bool _initialized = false;
		public string ConnectURL => _session.URI;
		public string SettingsFilename => SettingsFilenameString;

		private AppEntry _selectedWallet;
		private WalletConnectSession _session;

		public WalletConnect()
		{
		}

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
				Debug.LogError($"WalletConnect: Could not initialize because settings are " + typeStr);
			}
		}

		public void Update()
		{
			_transport?.Update();
		}

		public UniTask Quit()
		{
			return SaveOrDisconnect();
		}

		public async void Dispose()
		{
			await SaveOrDisconnect();
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

		public async UniTask<WCSessionData> Connect()
		{
			TeardownEvents();
			var savedSession = SessionSaveHandler.GetSavedSession();
			Logger.AddLog(PackageInfo.Version);

			if (_session != null)
			{
				if (savedSession != null)
				{
					if (_session.KeyData != savedSession.Key)
					{
						if (_session.Status.IsAny(WalletConnectStatus.SessionOrWalletConnected))
						{
							await _session.DisconnectSession();
						}
						else if (_session.Status == WalletConnectStatus.TransportConnected)
						{
							await _session.Transport.Close();
						}
					}
					else if (!_session.Status.IsAny(WalletConnectStatus.SessionOrWalletConnected) && !_session.Connecting)
					{
						return await CompleteConnect();
					}
					else
					{
						Debug.Log("Nothing to do, we are already connected and session key did not change");
						return null;
					}
				}
				else if (_session.Status.IsAny(WalletConnectStatus.SessionOrWalletConnected))
				{
					Debug.Log("We have old session connected, but no saved session. Disconnecting.");
					await _session.DisconnectSession();
				}
				else if (_session.Status == WalletConnectStatus.TransportConnected)
				{
					Debug.Log("We have transport connected, but no saved session. Closing Transport.");
					await _session.Transport.Close();
				}
				else if (_session.Connecting)
				{
					Debug.Log("Session connection is in progress. Connect request ignored.");
					return null;
				}
			}

			InitializeUnitySession(savedSession);

			return await CompleteConnect();
		}

		private void CheckIfSessionCreated()
		{
			if (_session == null)
			{
				throw new InvalidDataException("Session was not initialized yet, first connect your wallet connect");
			}
		}

		private void InitializeUnitySession(SavedSession savedSession = null, ICipher cipher = null)
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

		public void OpenMobileWallet(AppEntry selectedWallet)
		{
			_selectedWallet = selectedWallet;

			OpenMobileWallet();
		}

		public void OpenDeepLink(AppEntry selectedWallet)
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
					"You must use OpenMobileWallet(AppEntry) or set _selectedWallet on iOS!");
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
			if (!_session.Status.IsAny(WalletConnectStatus.SessionOrWalletConnected))
			{
				Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
				               "\nWait for ActiveSession.ReadyForUserPrompt to be true");
				return;
			}

		#if UNITY_ANDROID
			Application.OpenURL(ConnectURL);
		#elif UNITY_IOS
			if (_selectedWallet == null)
			{
				throw new NotImplementedException(
					"You must use OpenDeepLink(AppEntry) or set _selectedWallet on iOS!");
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

		private void SetupEvents()
		{
			if (_session == null)
			{
				Debug.LogError("Trying to setup events on null session");
				return;
			}

			_session.OnSessionDisconnect += HandleSessionDisconnect;
			_session.OnSessionCreated += HandleSessionCreated;
			_session.OnSessionResumed += HandleSessionResumed;
		}

		private void TeardownEvents()
		{
			if (_session == null)
			{
				return;
			}

			_session.OnSessionDisconnect -= HandleSessionDisconnect;
			_session.OnSessionCreated -= HandleSessionCreated;
			_session.OnSessionResumed -= HandleSessionResumed;
		}

		private void HandleSessionResumed(object sender, WalletConnectSession e)
		{
			
		}

		private void HandleSessionCreated(object sender, WalletConnectSession e)
		{
			var sessionToSave = _session.GetSavedSession();
			SessionSaveHandler.SaveSession(sessionToSave);
		}

		private async UniTask<WCSessionData> CompleteConnect()
		{
			SetupDefaultWallet().Forget();
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

		private async void HandleSessionDisconnect(object sender, EventArgs e)
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

			var supportedWallets = await WalletDownloadHelper.FetchWalletList(false);

			var wallet =
				supportedWallets.Values.FirstOrDefault(a =>
					string.Equals(a.name, _settings.DefaultWallet.GetWalletName(), StringComparison.InvariantCultureIgnoreCase));

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

			if (!_session.Status.IsAny(WalletConnectStatus.SessionOrWalletConnected))
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