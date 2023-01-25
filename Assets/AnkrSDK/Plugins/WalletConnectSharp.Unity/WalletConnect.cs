using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnkrSDK.Metadata;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Network;
using AnkrSDK.WalletConnectSharp.Unity.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink;
using AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink.Helpers;
using AnkrSDK.WalletConnectSharp.Unity.Network;
using AnkrSDK.WalletConnectSharp.Unity.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Logger = AnkrSDK.InternalUtils.Logger;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public class WalletConnect : IQuittableComponent, IPausableComponent, IDisposable
	{
		private readonly NativeWebSocketTransport _transport = new NativeWebSocketTransport();
		private WalletConnectSettingsSO _settings;
		private bool _initialized = false;

		private readonly WalletConnectEventWithSessionData _connectedEventSession = new WalletConnectEventWithSessionData();
		private readonly WalletConnectEventWithSession _disconnectedEvent = new WalletConnectEventWithSession();
		private readonly WalletConnectEventWithSession _newSessionConnected = new WalletConnectEventWithSession();
		private readonly WalletConnectEventWithSession _resumedSessionConnected = new WalletConnectEventWithSession();

		public event Action ConnectionStarted;
		public event Action SessionUpdated;
		public NativeWebSocketTransport Transport => _transport;
		public string ConnectURL => _session.URI;

		private AppEntry _selectedWallet;
		private WalletConnectUnitySession _session;

		public WalletConnectUnitySession Session
		{
			get => _session;
			private set
			{
				Debug.Log("Active Session Changed");
				_session = value;
				SessionUpdated?.Invoke();
			}
		}

		public WalletConnectEventWithSessionData ConnectedEvent => _connectedEventSession;

		public WalletConnect()
		{
		}

		public void Initialize(string settingsFileName = "WalletConnectSettings")
		{
			_settings = Resources.Load<WalletConnectSettingsSO>(settingsFileName);
			if (_settings != null)
			{
				_initialized = true;
			}
			else
			{
				Debug.LogError($"Could not initialize because {settingsFileName} not found in resources");
			}
		}

		public Task Quit()
		{
			return SaveOrDisconnect().AsTask();
		}

		public async void Dispose()
		{
			await SaveOrDisconnect();
		}

		public Task OnApplicationPause(bool pauseStatus)
		{
			CheckIfInitialized();
			
			if (pauseStatus)
			{
				return SaveOrDisconnect().AsTask();
			}
			else if (SessionSaveHandler.IsSessionSaved() && _settings.AutoSaveAndResume)
			{
				return Connect();
			}

			return Task.CompletedTask;
		}

		public async Task<WCSessionData> Connect()
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
						if (_session.Connected)
						{
							await _session.Disconnect();
						}
						else if (_session.TransportConnected)
						{
							await _session.Transport.Close();
						}
					}
					else if (!_session.Connected && !_session.Connecting)
					{
						if (!_session.Disconnected)
						{
							return await CompleteConnect();
						}
					}
					else
					{
						Debug.Log("Nothing to do, we are already connected and session key did not change");
						return null;
					}
				}
				else if (_session.Connected)
				{
					Debug.Log("We have old session connected, but no saved session. Disconnecting.");
					await _session.Disconnect();
				}
				else if (_session.TransportConnected)
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

		public void InitializeUnitySession(SavedSession savedSession = null, ICipher cipher = null)
		{
			var appData = _settings.AppData;
			var customBridgeUrl = _settings.CustomBridgeUrl;
			var chainId = _settings.ChainId;
			
			Session = savedSession != null
				? WalletConnectUnitySession.RestoreWalletConnectSession(savedSession, this, _transport)
				: WalletConnectUnitySession.GetNewWalletConnectSession(appData, this, customBridgeUrl, _transport,
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
			if (!_session.ReadyForUserPrompt)
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

			await _session.Disconnect();

			if (connectNewSession)
			{
				await _session.Connect();
			}
		}

		private void CheckIfInitialized()
		{	
			if (_settings == null)
			{
				throw new InvalidOperationException("WalletConnect object was not initialized. Call Initialize() method first");
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
			_resumedSessionConnected?.Invoke(e as WalletConnectUnitySession ?? _session);
		}

		private void HandleSessionCreated(object sender, WalletConnectSession e)
		{
			_newSessionConnected?.Invoke(e as WalletConnectUnitySession ?? _session);

			var sessionToSave = _session.GetSavedSession();
			SessionSaveHandler.SaveSession(sessionToSave);
		}

		private async Task<WCSessionData> CompleteConnect()
		{
			SetupDefaultWallet().Forget();
			SetupEvents();

			ConnectionStarted?.Invoke();
			var tries = 0;
			var connectSessionRetryCount = _settings.ConnectSessionRetryCount;
			while (tries < connectSessionRetryCount)
			{
				Debug.Log($"Trying to connect session. Try : {tries}");
				try
				{
					var sessionData = await _session.WaitForSessionToConnectAsync();

					_connectedEventSession?.Invoke(sessionData);
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
			_disconnectedEvent?.Invoke(_session);

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

			if (!_session.Connected)
			{
				return UniTask.CompletedTask;
			}

			if (!_settings.AutoSaveAndResume)
			{
				return _session.Disconnect().AsUniTask();
			}

			var sessionToSave = _session.GetSavedSession();
			SessionSaveHandler.SaveSession(sessionToSave);

			return _session.Transport.Close().AsUniTask();
		}
	}
}