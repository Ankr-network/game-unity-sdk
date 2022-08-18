using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using AnkrSDK.WebGL.DTO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WebGl
{
	public class WebGLConnect : MonoBehaviour
	{
		[SerializeField] private WebGL.SupportedWallets _defaultWallet = WebGL.SupportedWallets.None;
		[SerializeField] private NetworkName _defaultNetwork = NetworkName.Rinkeby;
		[SerializeField] private bool _connectOnAwake;
		[SerializeField] private bool _connectOnStart = true;

		private UniTaskCompletionSource<WebGL.SupportedWallets> _walletCompletionSource;
		public WebGL.WebGLWrapper Session { get; private set; }
		public Action OnNeedPanel;
		public Action<WebGL.WebGLWrapper> OnConnect;

	#if UNITY_WEBGL
		private async void Awake()
		{
			if (_connectOnAwake)
			{
				await Initialize();
			}
		}

		private async void Start()
		{
			if (_connectOnStart)
			{
				await Initialize();
			}
		}
	#endif

		private async Task Initialize()
		{
			DontDestroyOnLoad(this);
			Session = new WebGL.WebGLWrapper();
			await Connect();
		}

		private async UniTask Connect()
		{
			var wallet = _defaultWallet;
			if (wallet == WebGL.SupportedWallets.None)
			{
				OnNeedPanel?.Invoke();
				_walletCompletionSource = new UniTaskCompletionSource<WebGL.SupportedWallets>();
				wallet = await _walletCompletionSource.Task;
			}

			await Session.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_defaultNetwork));
			OnConnect?.Invoke(Session);
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return Session.GetWalletsStatus();
		}

		public void SetWallet(WebGL.SupportedWallets wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_defaultNetwork = network;
		}
	}
}