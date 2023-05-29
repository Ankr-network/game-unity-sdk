using System;
using System.Linq;
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
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
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
		private readonly WebGLCommunicationProtocol _protocol;

		public string SettingsFilename => SettingsFilenameStr;
		public string WalletName => _selectedWallet.HasValue ? _selectedWallet.Value.ToString() : "";

		public WebGLConnect()
		{
			_protocol = new WebGLCommunicationProtocol();
		}

		public void Initialize(ScriptableObject settings)
		{
			_protocol.StartReceiveCycle().Forget();
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
			await ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_network));

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

		public async UniTask<WalletsStatus> GetWalletsStatus()
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetWalletsStatus(id);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var payload = JsonConvert.DeserializeObject<WalletsStatus>(answer.payload);
				return payload;
			}

			throw new Exception(answer.payload);
		}

		public void SetWallet(Wallet wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_network = network;
		}

		public async UniTask<string> SendTransaction(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.SendTransaction(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> GetContractData(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.GetContractData(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<HexBigInteger> EstimateGas(TransactionData transaction)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(transaction);
			WebGLInterlayer.EstimateGas(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return new HexBigInteger(answer.payload);
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			var id = _protocol.GenerateId();

			WebGLInterlayer.SignMessage(id, JsonConvert.SerializeObject(signProps));

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return answer.payload;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<string> GetDefaultAccount(string network = null)
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetAddresses(id);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var addresses = JsonConvert.DeserializeObject<string[]>(answer.payload);
				return addresses.First();
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<BigInteger> GetChainId()
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.RequestChainId(id);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var payload = JsonConvert.DeserializeObject<WebGLCallAnswer<HexBigInteger>>(answer.payload);
				return payload.Result.Value;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<Transaction> GetTransaction(string transactionHash)
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetTransaction(id, transactionHash);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var transactionData = JsonConvert.DeserializeObject<Transaction>(answer.payload);
				return transactionData;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask AddChain(EthChainData networkData)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.AddChain(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public async UniTask UpdateChain(EthUpdateChainData networkData)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.AddChain(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public async UniTask SwitchChain(EthChain networkData)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(networkData);
			WebGLInterlayer.SwitchChain(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
		}

		public async UniTask<TReturnType> CallMethod<TReturnType>(WebGLCallObject callObject)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(callObject);
			WebGLInterlayer.CallMethod(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var callAnswer = JsonConvert.DeserializeObject<WebGLCallAnswer<TReturnType>>(answer.payload);

				return callAnswer.Result;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			var id = _protocol.GenerateId();
			var payload = JsonConvert.SerializeObject(filters);
			WebGLInterlayer.GetEvents(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				var logs = JsonConvert.DeserializeObject<FilterLog[]>(answer.payload);
				return logs;
			}

			throw new Exception(answer.payload);
		}

		public async UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			var id = _protocol.GenerateId();
			WebGLInterlayer.GetTransactionReceipt(id, transactionHash);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Success)
			{
				return JsonConvert.DeserializeObject<TransactionReceipt>(answer.payload);
			}

			throw new Exception(answer.payload);
		}

		private async UniTask ConnectTo(Wallet wallet, EthereumNetwork chain)
		{
			var id = _protocol.GenerateId();
			var connectionProps = new ConnectionProps
			{
				wallet = wallet.ToString(),
				chain = chain
			};

			var payload = JsonConvert.SerializeObject(connectionProps);
			WebGLInterlayer.CreateProvider(id, payload);

			var answer = await _protocol.WaitForAnswer(id);

			if (answer.status == WebGLMessageStatus.Error)
			{
				throw new Exception(answer.payload);
			}
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