using System;
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
		[SerializeField, Tools.CustomDropdown(typeof(SupportedWallets), "GetWebGLWallets")]
		private Wallet _defaultWallet = Wallet.None;
		[SerializeField] private NetworkName _defaultNetwork = NetworkName.Rinkeby;
		[SerializeField] private bool _connectOnAwake;
		[SerializeField] private bool _connectOnStart = true;

		private UniTaskCompletionSource<Wallet> _walletCompletionSource;
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
			if (wallet == Wallet.None)
			{
				OnNeedPanel?.Invoke();
				_walletCompletionSource = new UniTaskCompletionSource<Wallet>();
				wallet = await _walletCompletionSource.Task;
			}

			await Connect(wallet);
		}
		
		public async UniTask Connect(Wallet wallet)
		{
			await Session.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_defaultNetwork));
			OnConnect?.Invoke(Session);
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return Session.GetWalletsStatus();
		}

		public void SetWallet(Wallet wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_defaultNetwork = network;
		}
	}
}