using System;
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
		public WebGL.WebGLWrapper SessionWrapper { get; private set; }
		public Action OnNeedPanel;
		public Action<WebGL.WebGLWrapper> OnConnect;

	#if !UNITY_WEBGL || UNITY_EDITOR
		private void Awake()
		{
			gameObject.SetActive(false);
		}
	#else
		private void Awake()
		{
			SessionWrapper = new WebGL.WebGLWrapper();
			if (_connectOnAwake)
			{
				Initialize().Forget();
			}
		}

		private void Start()
		{
			if (_connectOnStart)
			{
				Initialize().Forget();
			}
		}
	#endif

		private UniTask Initialize()
		{
			DontDestroyOnLoad(this);
			return Connect();
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
			await SessionWrapper.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_defaultNetwork));
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
			_defaultNetwork = network;
		}
	}
}