using System;
using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Data;
using MirageSDK.Utils;
using MirageSDK.WebGL.DTO;
using MirageSDK.WebGL.DTO.JsonRpc;
using MirageSDK.WebGL.Infrastructure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Unity.Metamask;
using Nethereum.Unity.Rpc;
using Newtonsoft.Json;
using UnityEngine;
using TransactionData = MirageSDK.WebGL.DTO.TransactionData;

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
			var result = new WalletsStatus();
			result.Add(Wallet.Metamask, _isConnected);
			return result;
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

		public UniTask<HexBigInteger> EstimateGas(TransactionData transaction)
		{

		}

		public UniTask<string> Sign(DataSignaturePropsDTO signProps)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<string> GetDefaultAccount()
		{
			throw new System.NotImplementedException();
		}

		public UniTask<BigInteger> GetChainId()
		{
			throw new System.NotImplementedException();
		}

		public UniTask<Transaction> GetTransaction(string transactionHash)
		{
			throw new System.NotImplementedException();
		}

		public UniTask AddChain(EthChainData networkData)
		{
			throw new System.NotImplementedException();
		}

		public UniTask UpdateChain(EthUpdateChainData networkData)
		{
			throw new System.NotImplementedException();
		}

		public UniTask SwitchChain(EthChain networkData)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<TReturnType> CallMethod<TReturnType>(WebGLCallObject callObject)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<FilterLog[]> GetEvents(NewFilterInput filters)
		{
			throw new System.NotImplementedException();
		}

		public UniTask<TransactionReceipt> GetTransactionReceipt(string transactionHash)
		{
			throw new System.NotImplementedException();
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

		private async UniTask GetBlockNumber()
		{
			var blockNumberRequest = new EthBlockNumberUnityRequest(GetUnityRpcRequestClientFactory());
			await blockNumberRequest.SendRequest().ToUniTask();
			Debug.Log($"WebGL Nethereum: block number received {blockNumberRequest.Result}");
		}

		private IUnityRpcRequestClientFactory GetUnityRpcRequestClientFactory()
		{
			if (MetamaskWebglInterop.IsMetamaskAvailable())
			{
				return new MetamaskWebglCoroutineRequestRpcClientFactory(_selectedAccountAddress, null, 1000);
			}

			PrintError("Metamask is not available, please install it");
			return null;
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
			var operationId = GenerateOperationId();
			var completionSource = _completionSourceMap.CreateCompletionSource<EthResponse>(operationId);
			try
			{
				var jsonRequest = JsonConvert.SerializeObject(request);
				MetamaskWebglInterop.RequestRpcClientCallback(responseJson =>
				{
					//response is the json of the JSON-RPC 2.0 response
					try
					{
						var responseObj = JsonConvert.DeserializeObject<EthResponse>(responseJson);
						if (responseObj.IsError)
						{
							Debug.LogError($"JSON-RPC error: {responseObj.Error}");
							_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
						}
						else
						{
							_completionSourceMap.TrySetResult(operationId, responseObj);
						}
					}
					catch (Exception e)
					{
						PrintError($"WebGL Nethereum SendTransaction error {e}");
						_completionSourceMap.TrySetCanceled<EthResponse>(operationId);
					}

				}, jsonRequest);

				var result = await completionSource.Task;
				return result;
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