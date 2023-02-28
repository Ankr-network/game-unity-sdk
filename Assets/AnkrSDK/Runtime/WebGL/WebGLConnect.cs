using System;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnect.VersionShared;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WebGL
{
	public class WebGLConnect : IWalletConnectable
	{
		private const string SettingsFilenameStr = "WebGLConnectSettings";
		
		public Action OnNeedPanel;
		public Action<WebGLWrapper> OnConnect;
		public WebGLWrapper SessionWrapper { get; private set; }
		private UniTaskCompletionSource<Wallet> _walletCompletionSource;
		private WebGLConnectSettingsSO _settings;
		private NetworkName _network;
		private bool _initialized;

		public string SettingsFilename => SettingsFilenameStr;

		public WebGLConnect()
		{
			
		}

		public void Initialize(ScriptableObject settings)
		{
			SessionWrapper = new WebGL.WebGLWrapper();
			_settings = settings as WebGLConnectSettingsSO;
			if (_settings != null)
			{
				_initialized = true;
				_network = _settings.DefaultNetwork;
			}
			else
			{
				var typeStr = settings == null ? "null" : settings.GetType().Name;
				Debug.LogError($"WalletConnect: Could not initialize because settings are " + typeStr);
			}
		}

		public async UniTask Connect()
		{
			var wallet = _settings.DefaultWallet;
			if (wallet == Wallet.None)
			{
				OnNeedPanel?.Invoke();
				_walletCompletionSource = new UniTaskCompletionSource<Wallet>();
				wallet = await _walletCompletionSource.Task;
			}

			await SessionWrapper.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_network));
			
			OnConnect?.Invoke(SessionWrapper);
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return SessionWrapper.GetWalletsStatus();
		}

		public void SetWallet(Wallet wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_network = network;
		}
	}
}