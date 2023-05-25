using System;
using MirageSDK.Data;
using MirageSDK.Utils;
using MirageSDK.WalletConnect.VersionShared;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Core.StatusEvents;
using UnityEngine;

namespace MirageSDK.WebGL
{
	public class WebGLConnect : IWalletConnectable, IWalletConnectStatusHolder
	{
		private const string SettingsFilenameStr = "WebGLConnectSettings";
		public event Action<WalletConnectTransitionBase> SessionStatusUpdated;
		public WalletConnectStatus Status => _status;

		private WalletConnectStatus _status = WalletConnectStatus.Uninitialized;
		private WebGLWrapper _sessionWrapper;
		private UniTaskCompletionSource<Wallet> _walletCompletionSource;
		private WebGLConnectSettingsSO _settings;
		private NetworkName _network;

		public string SettingsFilename => SettingsFilenameStr;

		public WebGLConnect()
		{

		}

		public void Initialize(ScriptableObject settings)
		{
			_sessionWrapper = new WebGL.WebGLWrapper();
			_settings = settings as WebGLConnectSettingsSO;
			if (_settings != null)
			{
				_network = _settings.DefaultNetwork;
				_status = WalletConnectStatus.DisconnectedNoSession;
			}
			else
			{
				var typeStr = settings == null ? "null" : settings.GetType().Name;
				Debug.LogError($"WalletConnect: Could not initialize because settings are " + typeStr);
			}
		}

		public async UniTask Connect()
		{
			UpdateStatus(WalletConnectStatus.TransportConnected);
			var wallet = _settings.DefaultWallet;
			if (wallet == Wallet.None)
			{
				_walletCompletionSource = new UniTaskCompletionSource<Wallet>();
				wallet = await _walletCompletionSource.Task;
			}

			UpdateStatus(WalletConnectStatus.SessionRequestSent);
			await _sessionWrapper.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_network));
			UpdateStatus(WalletConnectStatus.WalletConnected);
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return _sessionWrapper.GetWalletsStatus();
		}

		public void SetWallet(Wallet wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_network = network;
		}

		private void UpdateStatus(WalletConnectStatus newStatus)
		{
			var prevStatus = _status;
			_status = newStatus;
			if (newStatus != prevStatus)
			{
				var transition = TransitionDataFactory.CreateTransitionObj(prevStatus, newStatus, _sessionWrapper);
				SessionStatusUpdated?.Invoke(transition);
			}
		}
	}
}