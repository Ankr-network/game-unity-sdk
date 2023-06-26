using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AOT;
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
	public class WebGLNethereumCommunicationProtocol : IWebGLCommunicationProtocol
	{
		private static WebGLNethereumCommunicationProtocol _monoStateInstance;
		public delegate void GenericCallbackDelegate(string str);

		private readonly NamedCompletionSourceMap _completionSourceMap = new NamedCompletionSourceMap();

		private readonly Queue<string> _requestOperationIdQueue = new Queue<string>();
		private bool _isConnected;
		private BigInteger _currentChainId;
		private string _selectedAccountAddress;
		private string _lastCalledRequest;

		public void Init()
		{
			_monoStateInstance = this;
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
					//EthereumEnabled, DisplayError
					MetamaskWebglInteropExtension.EnableEthereumRpcClientCallback(EthereumEnabledInteropCallback, DisplayErrorInteropCallback);
					await completionSource.Task;
				}
				else
				{
					LogError("Metamask is not available, please install it");
					completionSource.TrySetCanceled();
				}
			}
			catch (Exception e)
			{
				_completionSourceMap.TrySetCanceled(operationName);
				LogError($"Metamask initialization error {e}");
			}

		}

		[MonoPInvokeCallback(typeof(GenericCallbackDelegate))]
		private static void EthereumEnabledInteropCallback(string addressSelected)
		{
			_monoStateInstance.EthereumEnabled(addressSelected);
		}

		[MonoPInvokeCallback(typeof(GenericCallbackDelegate))]
		private static void DisplayErrorInteropCallback(string errorMessage)
		{
			_monoStateInstance.LogError(errorMessage);
		}

		public void Disconnect()
		{
			MetamaskWebglInterop.EthereumInitRpcClientCallback(DisconnectDummyCallback, DisconnectDummyCallback);

			_currentChainId = default;
			_selectedAccountAddress = default;
			_isConnected = false;
		}

		private static void DisconnectDummyCallback(string _)
		{

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
			var response = await Request<EthSendTransaction, EthResponse>(new EthSendTransaction(transactionData));
			var transactionHash = response.Result;
			if (!IsHexStringWith32Bytes(transactionHash))
			{
				Debug.LogError($"SendTransaction: transactionHash wrong format {transactionHash}");
			}
			return transactionHash;
		}

		public async UniTask<string> GetContractData(TransactionData transactionData)
		{
			var response = await Request<EthCallRequest, EthResponse>(new EthCallRequest(transactionData));
			return response.Result;
		}

		public async UniTask<HexBigInteger> EstimateGas(TransactionData transactionData)
		{
			var response = await Request<EthEstimateGas, EthResponse>(new EthEstimateGas(transactionData));
			return new HexBigInteger(response.Result);
		}

		public async UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			var address = signProps.address;
			var message = signProps.message.ToEthSignableHex();
			var response = await Request<EthSign, EthResponse>(new EthSign(address, message));
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
				MetamaskWebglInteropExtension.GetChainIdRpcClientCallback(ChainChangedInteropCallback, DisplayErrorInteropCallback);
				var result = await completionSource.Task;
				return new HexBigInteger(result).Value;
			}
			catch (Exception e)
			{
				_completionSourceMap.TrySetCanceled(operationName);
				LogError($"Error in GetChain: {e}");
				return 0;
			}
		}

		[MonoPInvokeCallback(typeof(GenericCallbackDelegate))]
		private static void ChainChangedInteropCallback(string chainId)
		{
			_monoStateInstance.ChainChanged(chainId);

			const string operationName = "GetChainId";
			if (_monoStateInstance._completionSourceMap.HasCompletionSourceFor(operationName))
			{
				_monoStateInstance._completionSourceMap.TrySetResult(operationName, chainId);
			}
		}

		public async UniTask<Transaction> GetTransaction(string transactionHash)
		{
			if (!IsHexStringWith32Bytes(transactionHash))
			{
				Debug.LogError($"GetTransaction: transactionHash wrong format {transactionHash}");
			}

			var response = await Request<EthGetTransactionByHash, EthResponse>(new EthGetTransactionByHash(transactionHash));
			var transactionJson = response.Result;

			try
			{
				return JsonConvert.DeserializeObject<Transaction>(transactionJson);
			}
			catch (JsonReaderException jsonReaderException)
			{
				Debug.LogError($"GetTransaction: json {transactionJson} deserialization exception {jsonReaderException}");
				return null;
			}
		}

		public async UniTask AddChain(EthChainData networkData)
		{
			await Request<WalletAddEthChain, EthResponse>(new WalletAddEthChain(networkData));
		}

		public UniTask UpdateChain(EthUpdateChainData networkData)
		{
			throw new NotImplementedException("wallet_updateChain is not implemented for Metamask");
		}

		public async UniTask SwitchChain(EthChain networkData)
		{
			await Request<WalletSwitchEthChain, EthResponse>(new WalletSwitchEthChain(networkData));
		}

		//measured in wei
		public async UniTask<BigInteger> GetBalance()
		{
			var address = await GetDefaultAccount();
			var response = await Request<EthGetBalance, EthResponse>(new EthGetBalance(address));
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return -1;
			}
			return new HexBigInteger(response.Result).Value;
		}

		public async UniTask<BigInteger> GetBlockNumber()
		{
			var response = await Request<EthBlockNumber, EthResponse>(new EthBlockNumber());
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return -1;
			}
			return new HexBigInteger(response.Result).Value;
		}

		public async UniTask<BigInteger> GetBlockTransactionCount(string blockNumber)
		{
			var response = await Request<EthGetBlockTransactionCount, EthResponse>(new EthGetBlockTransactionCount(blockNumber));
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return -1;
			}
			return new HexBigInteger(response.Result).Value;
		}

		public async UniTask<TResultType> GetBlock<TResultType>(string blockNumber, bool returnTransactionObjects)
		{
			var response = await Request<EthGetBlockByNumber, EthResponse>(new EthGetBlockByNumber(blockNumber, returnTransactionObjects));
			return ExtractResultFromEthResponse<TResultType>(response);
		}

		public async UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			var response = await Request<EthGetLogs, EthResponse>(new EthGetLogs(filters));
			return ExtractResultFromEthResponse<FilterLog[]>(response);
		}
		public async UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			if (!IsHexStringWith32Bytes(transactionHash))
			{
				Debug.LogError($"GetTransactionReceipt: transactionHash wrong format {transactionHash}");
			}

			TransactionReceipt resultReceipt = null;
			EthGetTransactionReceiptResponse resultResponse = null;
			while (resultReceipt == null)
			{
				var response = await Request<EthGetTransactionReceipt, EthGetTransactionReceiptResponse>(new EthGetTransactionReceipt(transactionHash));
				resultReceipt = response.Result;

				if (resultReceipt != null)
				{
					resultResponse = response;
				}
				else
				{
					const int millisecondsRequestDelay = 500;
					await UniTask.Delay(TimeSpan.FromMilliseconds(millisecondsRequestDelay));
				}
			}

			string jsonString = JsonConvert.SerializeObject(resultReceipt, Formatting.Indented);
			Debug.Log($"TransactionReceipt received: {jsonString} response: {resultResponse}");
			return resultReceipt;
		}

		private static bool IsHexStringWith32Bytes(string input)
		{
			// Regular expression pattern to match a hexadecimal string with 64 characters (32 bytes)
			string pattern = @"^0x[A-Fa-f0-9]{64}$";

			// Match input against the pattern
			return Regex.IsMatch(input, pattern);
		}

		private T ExtractResultFromEthResponse<T>(EthResponse response)
		{
			bool noResult = response == null || string.IsNullOrWhiteSpace(response.Result);
			if (noResult)
			{
				return default;
			}

			try
			{
				return JsonConvert.DeserializeObject<T>(response.Result);
			}
			catch (JsonReaderException e)
			{
				Debug.LogError($"JsonReaderException while trying to deserialize {response.Result} StackTrace: {e.StackTrace}");
				return default;
			}
		}

		private void EthereumEnabled(string addressSelected)
		{
			const string operationName = "ConnectTo";
			try
			{
				if (!_isConnected)
				{
					//callbacks: NewAccountSelected, ChainChanged
					MetamaskWebglInterop.EthereumInitRpcClientCallback(NewAccountSelectedInteropCallback, ChainChangedInteropCallback);

					//callbacks: ChainChanged, DisplayError
					MetamaskWebglInteropExtension.GetChainIdRpcClientCallback(ChainChangedInteropCallback, DisplayErrorInteropCallback);
					_isConnected = true;
				}

				_selectedAccountAddress = addressSelected;

				_completionSourceMap.TrySetResult(operationName, null);

			}
			catch (Exception e)
			{
				LogError($"Metamask initialization error {e}");
				_completionSourceMap.TrySetCanceled(operationName);
			}
		}

		[MonoPInvokeCallback(typeof(GenericCallbackDelegate))]
		private static void NewAccountSelectedInteropCallback(string accountAddress)
		{
			_monoStateInstance.NewAccountSelected(accountAddress);
		}

		private void NewAccountSelected(string accountAddress)
		{
			_selectedAccountAddress = accountAddress;
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
				LogError(ex.Message);
			}
		}

		private void LogError(string errorMessage)
		{
			Debug.LogError($"WebGL Nethereum error: {errorMessage} last called request: " + _lastCalledRequest);
		}

		private string GenerateOperationId()
		{
			var id = Guid.NewGuid().ToString();
			return id;
		}

		private async UniTask<TResponse> Request<TRequest, TResponse>(TRequest request)
			where TRequest: JsonRpcRequest where TResponse: JsonRpcResponse
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
			var completionSource = _completionSourceMap.CreateCompletionSource<TResponse>(operationId);

			bool operationIdEnqueued = false;
			try
			{
				var jsonRequest = request != null ? JsonConvert.SerializeObject(request) : "{}";
				MetamaskWebglInterop.RequestRpcClientCallback(RpcResponseInteropCallback, jsonRequest);

				operationIdEnqueued = true;
				_requestOperationIdQueue.Enqueue(operationId);

				return await completionSource.Task;
			}
			catch (Exception e)
			{
				LogError($"WebGL Nethereum Request error {e}");
				_completionSourceMap.TrySetCanceled(operationId);
				if (operationIdEnqueued)
				{
					_requestOperationIdQueue.Dequeue();
				}
				return null;
			}
		}

		[MonoPInvokeCallback(typeof(GenericCallbackDelegate))]
		private static void RpcResponseInteropCallback(string jsonResponse)
		{
			_monoStateInstance.HandleRpcResponse(jsonResponse);
		}

		private void HandleRpcResponse(string jsonResponse)
		{
			if (_requestOperationIdQueue.Count == 0)
			{
				throw new InvalidOperationException("There is no operation id in the response operation id queue");
			}

			var operationId = _requestOperationIdQueue.Peek();
			//response is the json of the JSON-RPC 2.0 response
			try
			{
				var resultType = _completionSourceMap.GetOperationResultType(operationId);
				var responseObj = JsonConvert.DeserializeObject(jsonResponse, resultType) as JsonRpcResponse;

				if (responseObj == null)
				{
					Debug.LogError($"JSON-RPC deserialization error for: {jsonResponse}");
				}
				else if (responseObj.IsError)
				{
					Debug.LogError($"JSON-RPC error: {responseObj.Error?.Message}");
					_completionSourceMap.TrySetCanceled(operationId);
				}
				else
				{
					_completionSourceMap.TrySetResult(operationId, responseObj);
				}

				_requestOperationIdQueue.Dequeue();
			}
			catch (JsonReaderException jsonReaderException)
			{
				LogError($"WebGL Nethereum Request json {jsonResponse} deserialization error {jsonReaderException}");
				_completionSourceMap.TrySetCanceled(operationId);
				_requestOperationIdQueue.Dequeue();
			}
			catch (Exception e)
			{
				LogError($"WebGL Nethereum Request error {e}");
				_completionSourceMap.TrySetCanceled(operationId);
				_requestOperationIdQueue.Dequeue();
			}
		}
	}
}