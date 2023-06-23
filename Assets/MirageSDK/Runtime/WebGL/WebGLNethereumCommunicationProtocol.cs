using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
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
		private const string CommunicatorGoName = "WebGLNethereumCommunicator";
		private static string GoUid = null;
		private WebGLNethereumMessageBridge _webglNethereumMessageBridge;
		private readonly NamedCompletionSourceMap _completionSourceMap = new NamedCompletionSourceMap();

		private readonly Queue<string> _requestOperationIdQueue = new Queue<string>();
		private bool _isConnected;
		private BigInteger _currentChainId;
		private string _selectedAccountAddress;
		private string _lastCalledRequest;

		private string MessageBridgeGoName => _webglNethereumMessageBridge.gameObject.name;

		public void Init()
		{
			if (_webglNethereumMessageBridge == null)
			{
				WebGLNethereumMessageBridge messageBridge = null;

				var communicatorGo = GoUid != null ? GameObject.Find(CommunicatorGoName + "_" + GoUid) : null;
				if (communicatorGo == null)
				{
					GoUid = Guid.NewGuid().ToString().Substring(0, 4);
					communicatorGo = new GameObject(CommunicatorGoName + "_" + GoUid);
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
					MetamaskWebglInterop.EnableEthereum(MessageBridgeGoName, names.FirstCallbackName, names.SecondCallbackName);
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
			var transactionHash = response.Result;
			if (!IsHexStringWith32Bytes(transactionHash))
			{
				Debug.LogError($"SendTransaction: transactionHash wrong format {transactionHash}");
			}
			return transactionHash;
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
				MetamaskWebglInterop.GetChainId(MessageBridgeGoName, names.FirstCallbackName, names.SecondCallbackName);
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
			if (!IsHexStringWith32Bytes(transactionHash))
			{
				Debug.LogError($"GetTransaction: transactionHash wrong format {transactionHash}");
			}

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
			var response = await Request<EthBlockNumber>(new EthBlockNumber());
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
			if (!IsHexStringWith32Bytes(transactionHash))
			{
				Debug.LogError($"GetTransactionReceipt: transactionHash wrong format {transactionHash}");
			}

			var response = await Request(new EthGetTransactionReceipt(transactionHash));
			var receipt = ExtractResult<TransactionReceipt>(response);
			string jsonString = JsonConvert.SerializeObject(receipt, Formatting.Indented);
			Debug.Log($"TransactionReceipt received: {jsonString}");
			return receipt;
		}

		private static bool IsHexStringWith32Bytes(string input)
		{
			// Regular expression pattern to match a hexadecimal string with 64 characters (32 bytes)
			string pattern = @"^0x[A-Fa-f0-9]{64}$";

			// Match input against the pattern
			return Regex.IsMatch(input, pattern);
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
					var names = _webglNethereumMessageBridge.EthereumInitCallbackNames;
					MetamaskWebglInterop.EthereumInit(MessageBridgeGoName, names.FirstCallbackName, names.SecondCallbackName);

					names = _webglNethereumMessageBridge.GetChainIdCallbackNames;
					//callbacks: ChainChanged, DisplayError
					MetamaskWebglInterop.GetChainId(MessageBridgeGoName, names.FirstCallbackName, names.SecondCallbackName);
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
			Debug.LogError($"WebGL Nethereum error: {errorMessage} last called request: " + _lastCalledRequest);
		}

		private string GenerateOperationId()
		{
			var id = Guid.NewGuid().ToString();
			return id;
		}

		private async UniTask<EthResponse> Request<TRequest>(TRequest request)
			where TRequest: JsonRpcRequest
		{
			if (request == null)
			{
				Debug.LogError("Request error: request is null");
				return null;
			}

			if (string.IsNullOrWhiteSpace(request.Method))
			{
				Debug.LogError("Request error: method is none for request " + request.GetType().Name);
				return null;
			}

			_lastCalledRequest = JsonConvert.SerializeObject(request);

			var operationId = GenerateOperationId();
			var completionSource = _completionSourceMap.CreateCompletionSource<EthResponse>(operationId);

			bool operationIdEnqueued = false;
			try
			{
				var jsonRequest = request != null ? JsonConvert.SerializeObject(request) : "{}";

				var names = _webglNethereumMessageBridge.RequestCallbackNames;

				operationIdEnqueued = true;
				MetamaskWebglInterop.Request(jsonRequest, MessageBridgeGoName, names.FirstCallbackName,
					names.SecondCallbackName);
				_requestOperationIdQueue.Enqueue(operationId);

				return await completionSource.Task;
			}
			catch (Exception e)
			{
				PrintError($"WebGL Nethereum Request error {e}");
				_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
				if (operationIdEnqueued)
				{
					_requestOperationIdQueue.Dequeue();
				}
				return null;
			}
		}

		void IWebGLNethereumCallbacksReceiver.HandleRpcResponse(string jsonResponse)
		{
			if (_requestOperationIdQueue.Count == 0)
			{
				throw new InvalidOperationException("There is no operation id in the response operation id queue");
			}

			var operationId = _requestOperationIdQueue.Peek();
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

				_requestOperationIdQueue.Dequeue();
			}
			catch (Exception e)
			{
				PrintError($"WebGL Nethereum Request error {e}");
				_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
				_requestOperationIdQueue.Dequeue();
			}
		}
	}
}