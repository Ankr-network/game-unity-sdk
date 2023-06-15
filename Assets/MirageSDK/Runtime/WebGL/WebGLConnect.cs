using System;
using System.Numerics;
using MirageSDK.Data;
using MirageSDK.Utils;
using MirageSDK.WalletConnect.VersionShared;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Infrastructure;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using MirageSDK.WalletConnectSharp.Core;
using MirageSDK.WalletConnectSharp.Core.StatusEvents;
using MirageSDK.WebGL.DTO;
using MirageSDK.WebGL.Infrastructure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;
using TransactionData = MirageSDK.WebGL.DTO.TransactionData;

namespace MirageSDK.WebGL
{
	public class WebGLConnect : IWalletConnectable, IWalletConnectStatusHolder, IWalletConnectTransitionDataProvider
	{
		private const string SettingsFilenameStr = "WebGLConnectSettings";
		public event Action<WalletConnectTransitionBase> SessionStatusUpdated;
		public event Action<string[]> OnAccountChanged;
		public WalletConnectStatus Status => _status;

		private WalletConnectStatus _status = WalletConnectStatus.Uninitialized;
		private WebGLWrapper _sessionWrapper;
		private UniTaskCompletionSource<Wallet> _walletCompletionSource;
		private WebGLConnectSettingsSO _settings;
		private NetworkName _network;
		private Wallet? _selectedWallet;
		private readonly IWebGLCommunicationProtocol _protocol;

		public string SettingsFilename => SettingsFilenameStr;
		public string WalletName => _selectedWallet.HasValue ? _selectedWallet.Value.ToString() : "";

		public WebGLConnect()
		{
			_protocol = new WebGLNethereumCommunicationProtocol();
		}

		public void Initialize(ScriptableObject settings)
		{
			_settings = settings as WebGLConnectSettingsSO;
			if (_settings != null)
			{
				_protocol.Init();
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
			await _protocol.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_network));

			var account = await GetDefaultAccount();
			OnAccountChanged?.Invoke(new [] {account});
			UpdateStatus(WalletConnectStatus.WalletConnected);
		}

		public UniTask CloseSession(bool connectNewSession = true)
		{
			_protocol.Disconnect();
			UpdateStatus(WalletConnectStatus.DisconnectedNoSession);
			return UniTask.CompletedTask;
		}

		public async UniTask<string> ReconnectSession()
		{
			await CloseSession();
			await Connect();
			return "";
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return _protocol.GetWalletsStatus();
		}

		public void SetWallet(Wallet wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_network = network;
		}

		public UniTask<string> SendTransaction(TransactionData transaction)
		{
			return _protocol.SendTransaction(transaction);
		}

		public UniTask<string> GetContractData(TransactionData transaction)
		{
			return _protocol.GetContractData(transaction);
		}

		public UniTask<HexBigInteger> EstimateGas(TransactionData transaction)
		{
			return _protocol.EstimateGas(transaction);
		}

		public UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			return _protocol.Sign(signProps);
		}

		public UniTask<string> GetDefaultAccount(string network = null)
		{
			return _protocol.GetDefaultAccount();
		}

		public UniTask<BigInteger> GetChainId()
		{
			return _protocol.GetChainId();
		}

		public UniTask<Transaction> GetTransaction(string transactionHash)
		{
			return _protocol.GetTransaction(transactionHash);
		}

		public UniTask AddChain(EthChainData networkData)
		{
			return _protocol.AddChain(networkData);
		}

		public UniTask UpdateChain(EthUpdateChainData networkData)
		{
			return _protocol.UpdateChain(networkData);
		}

		public UniTask SwitchChain(EthChain networkData)
		{
			return _protocol.SwitchChain(networkData);
		}

		public UniTask<TReturnType> CallMethod<TReturnType>(WebGLCallObject callObject)
		{
			return _protocol.CallMethod<TReturnType>(callObject);
		}

		public UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			return _protocol.GetEvents(filters);
		}

		public UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			return _protocol.GetTransactionReceipt(transactionHash);
		}

		private void UpdateStatus(WalletConnectStatus newStatus)
		{
			var prevStatus = _status;
			_status = newStatus;
			if (newStatus != prevStatus)
			{
				var transition = TransitionDataFactory.CreateTransitionObj(prevStatus, newStatus, this);
				SessionStatusUpdated?.Invoke(transition);
			}
		}
	}
}