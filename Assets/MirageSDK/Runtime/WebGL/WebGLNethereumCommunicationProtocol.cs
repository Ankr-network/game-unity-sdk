using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MirageSDK.Data;
using MirageSDK.Utils;
using MirageSDK.WalletConnect.VersionShared.Models;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;
using MirageSDK.WalletConnect.VersionShared.Utils;
using MirageSDK.WebGL.DTO;
using MirageSDK.WebGL.Infrastructure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Unity.Metamask;
using Newtonsoft.Json;
using UnityEngine;

namespace MirageSDK.WebGL
{
	public class WebGLNethereumCommunicationProtocol : IWebGLCommunicationProtocol, IWebGLNethereumCallbacksReceiver
	{
		private const string CommunicatorGoTag = "WebGLNethereumCommunicator";
		private WebGLNethereumMessageBridge _webglNethereumMessageBridge;
		private readonly NamedCompletionSourceMap _completionSourceMap = new NamedCompletionSourceMap();

		private bool _isConnected;
		private BigInteger _currentChainId;
		private string _selectedAccountAddress;

		private string MessageBridgeGoName => _webglNethereumMessageBridge.gameObject.name;

		public void Init()
		{
			if (_webglNethereumMessageBridge == null)
			{
				WebGLNethereumMessageBridge messageBridge = null;
				var communicatorGo = GameObject.FindWithTag(CommunicatorGoTag);
				if (communicatorGo == null)
				{
					communicatorGo = new GameObject(CommunicatorGoTag);
					communicatorGo.tag = CommunicatorGoTag;
					messageBridge = communicatorGo.AddComponent<WebGLNethereumMessageBridge>();
				}
				else
				{
					messageBridge = communicatorGo.GetComponent<WebGLNethereumMessageBridge>();
				}

				_webglNethereumMessageBridge = messageBridge;
			}

			_webglNethereumMessageBridge.SetProtocol(this);
		}

		public async UniTask ConnectTo(Wallet wallet, EthereumNetwork chain)
		{
			const string operationName = "ConnectTo";

			if (_completionSourceMap.HasCompletionSourceFor(operationName))
			{
				return;
			}

			var completionSource = _completionSourceMap.CreateCompletionSource(operationName);
			try
			{
				if (MetamaskWebglInterop.IsMetamaskAvailable())
				{
					var names = _webglNethereumMessageBridge.EnableEthereumCallbackNames;
					//EthereumEnabled, DisplayError
					MetamaskWebglInterop.EnableEthereum(MessageBridgeGoName, names.CallbackName, names.FallbackName);
					await completionSource.Task;
				}
				else
				{
					PrintError("Metamask is not available, please install it");
					completionSource.TrySetCanceled();
				}
			}
			catch (Exception e)
			{
				_completionSourceMap.TrySetCanceled(operationName);
				PrintError($"Metamask initialization error {e}");
			}

		}

		public void Disconnect()
		{
			const string DisconnectedCallback = "DisconnetedDummy";
			MetamaskWebglInterop.EthereumInit(MessageBridgeGoName, DisconnectedCallback, DisconnectedCallback);

			_currentChainId = default;
			_selectedAccountAddress = default;
			_isConnected = false;
		}

		public async UniTask<WalletsStatus> GetWalletsStatus()
		{
			return await Task.Run(() =>
			{
				var result = new WalletsStatus();
				result.Add(Wallet.Metamask, _isConnected);
				return result;
			});
		}

		public async UniTask<string> SendTransaction(TransactionData transactionData)
		{
			var response = await Request(new EthSendTransaction(transactionData));
			return response.Result;
		}

		public async UniTask<string> GetContractData(TransactionData transactionData)
		{
			var response = await Request(new EthCallRequest(transactionData));
			return response.Result;
		}

		public async UniTask<HexBigInteger> EstimateGas(TransactionData transactionData)
		{
			var response = await Request(new EthEstimateGas(transactionData));
			return new HexBigInteger(response.Result);
		}

		public async UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			var address = signProps.address;
			var message = signProps.message.ToEthSignableHex();
			var response = await Request(new EthSign(address, message));
			return response.Result;
		}

		public UniTask<string> GetDefaultAccount()
		{
			var address = MetamaskWebglInterop.GetSelectedAddress();
			return UniTask.FromResult(address);
		}

		public async UniTask<BigInteger> GetChainId()
		{
			const string operationName = "GetChainId";
			if (_completionSourceMap.HasCompletionSourceFor(operationName))
			{
				throw new InvalidOperationException(
					"You can not call another GetChainId before previous have finished");
			}

			var completionSource = _completionSourceMap.CreateCompletionSource<string>(operationName);

			try
			{
				var names = _webglNethereumMessageBridge.GetChainIdCallbackNames;
				//callbacks: ChainChanged, DisplayError
				MetamaskWebglInterop.GetChainId(MessageBridgeGoName, names.CallbackName, names.FallbackName);
				var result = await completionSource.Task;
				return new HexBigInteger(result).Value;
			}
			catch (Exception e)
			{
				_completionSourceMap.TrySetCanceled<string>(operationName);
				PrintError($"Error in GetChain: {e}");
				return 0;
			}
		}

		public async UniTask<Transaction> GetTransaction(string transactionHash)
		{
			var response = await Request(new EthGetTransactionByHash(transactionHash));
			var transactionJson = response.Result;
			return JsonConvert.DeserializeObject<Transaction>(transactionJson);
		}

		public async UniTask AddChain(EthChainData networkData)
		{
			await Request(new WalletAddEthChain(networkData));
		}

		public UniTask UpdateChain(EthUpdateChainData networkData)
		{
			throw new NotImplementedException("wallet_updateChain is not implemented for Metamask");
		}

		public async UniTask SwitchChain(EthChain networkData)
		{
			await Request(new WalletSwitchEthChain(networkData));
		}

		//measured in wei
		public async UniTask<BigInteger> GetBalance()
		{
			var address = await GetDefaultAccount();
			var response = await Request(new EthGetBalance(address));
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return -1;
			}
			return new HexBigInteger(response.Result).Value;
		}

		public async UniTask<BigInteger> GetBlockNumber()
		{
			var response = await Request<EthBlockNumber>(null);
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return -1;
			}
			return new HexBigInteger(response.Result).Value;
		}

		public async UniTask<BigInteger> GetBlockTransactionCount(string blockNumber)
		{
			var response = await Request(new EthGetBlockTransactionCount(blockNumber));
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return -1;
			}
			return new HexBigInteger(response.Result).Value;
		}

		public async UniTask<TResultType> GetBlock<TResultType>(string blockNumber, bool returnTransactionObjects)
		{
			var response = await Request(new EthGetBlockByNumber(blockNumber, returnTransactionObjects));
			return ExtractResult<TResultType>(response);
		}

		public async UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			var response = await Request(new EthGetLogs(filters));
			return ExtractResult<FilterLog[]>(response);
		}

		public async UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			var response = await Request(new EthGetTransactionReceipt(transactionHash));
			return ExtractResult<TransactionReceipt>(response);
		}

		private T ExtractResult<T>(EthResponse response)
		{
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return default;
			}

			return JsonConvert.DeserializeObject<T>(response.Result);
		}

		void IWebGLNethereumCallbacksReceiver.EthereumEnabled(string addressSelected)
		{
			const string operationName = "ConnectTo";
			try
			{
				if (!_isConnected)
				{
					//callbacks: NewAccountSelected, ChainChanged
					MetamaskWebglInterop.EthereumInitRpcClientCallback(
						accountAddress =>
						{
							_selectedAccountAddress = accountAddress;
						},
						chainId =>
						{
							ChainChanged(chainId);
						});

					var names = _webglNethereumMessageBridge.GetChainIdCallbackNames;
					//callbacks: ChainChanged, DisplayError
					MetamaskWebglInterop.GetChainId(MessageBridgeGoName, names.CallbackName, names.FallbackName);
					_isConnected = true;
				}

				_selectedAccountAddress = addressSelected;

				_completionSourceMap.TrySetResult(operationName);

			}
			catch (Exception e)
			{
				PrintError($"Metamask initialization error {e}");
				_completionSourceMap.TrySetCanceled(operationName);
			}
		}

		void IWebGLNethereumCallbacksReceiver.NewAccountSelected(string accountAddress)
		{
			_selectedAccountAddress = accountAddress;
		}

		void IWebGLNethereumCallbacksReceiver.ChainChanged(string chainId)
		{
			ChainChanged(chainId);

			const string operationName = "GetChainId";
			if (_completionSourceMap.HasCompletionSourceFor(operationName))
			{
				_completionSourceMap.TrySetResult<string>(operationName, chainId);
			}
		}

		private void ChainChanged(string chainId)
		{
			Debug.Log($"WebGL Nethereum: chain changed to {chainId}");
			_currentChainId = new HexBigInteger(chainId).Value;
			try
			{
				GetBlockNumber().Forget();
			}
			catch(Exception ex)
			{
				PrintError(ex.Message);
			}
		}

		void IWebGLNethereumCallbacksReceiver.DisplayError(string errorMessage)
		{
			PrintError(errorMessage);
		}

		private void PrintError(string errorMessage)
		{
			Debug.LogError($"WebGL Nethereum error: {errorMessage}");
		}

		private string GenerateOperationId()
		{
			var id = Guid.NewGuid().ToString();
			return id;
		}

		private async UniTask<EthResponse> Request<TRequest>(TRequest request)
			where TRequest: JsonRpcRequest
		{
			const int requestTimeout = 3;
			var operationId = GenerateOperationId();
			var completionSource = _completionSourceMap.CreateCompletionSource<EthResponse>(operationId);

			var timeout = TimeSpan.FromSeconds(requestTimeout);
			var timeoutCancellationTokenSource = new CancellationTokenSource(timeout);

			try
			{
				var jsonRequest = request != null ? JsonConvert.SerializeObject(request) : "{}";
				MetamaskWebglInterop.RequestRpcClientCallback(jsonResponse =>
				{
					//response is the json of the JSON-RPC 2.0 response
					try
					{
						var responseObj = JsonConvert.DeserializeObject<EthResponse>(jsonResponse);
						if (responseObj.IsError)
						{
							Debug.LogError($"JSON-RPC error: {responseObj.Error?.Message}");
							_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
						}
						else
						{
							_completionSourceMap.TrySetResult(operationId, responseObj);
						}
					}
					catch (Exception e)
					{
						PrintError($"WebGL Nethereum Request error {e}");
						_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
					}

				}, jsonRequest);

				var delayTask = UniTask.Delay(timeout, false, PlayerLoopTiming.Update, timeoutCancellationTokenSource.Token);
				await UniTask.WhenAny(completionSource.Task, delayTask);

				if (delayTask.Status.IsCompleted())
				{
					PrintError($"WebGL Nethereum Request timeout error ({requestTimeout} seconds)");
					_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
					return null;
				}

				return await completionSource.Task;
			}
			catch (Exception e)
			{
				PrintError($"WebGL Nethereum SendTransaction error {e}");
				_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
				return null;
			}
		}
	}
}