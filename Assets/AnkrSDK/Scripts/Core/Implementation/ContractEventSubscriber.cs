using System;
using System.Collections.Generic;
using System.Text;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Subscriptions;
using Nethereum.Web3;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{
	public delegate void OpenEventHandler();
	public delegate void ErrorEventHandler(string errorMsg);
	public delegate void CloseEventHandler(WebSocketCloseCode closeCode);
	
	public class ContractEventSubscriber : IClientRequestHeaderSupport
	{
		public event OpenEventHandler OnOpenHandler;
		public event ErrorEventHandler OnErrorHandler;
		public event CloseEventHandler OnCloseHandler;
		
		public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
		private WebSocket _transport;
		private bool _isCancellationRequested;

		private readonly string _wsUrl;
		private readonly Dictionary<string, IContractEventSubscription> _subscribers;
		private readonly EthLogsSubscriptionRequestBuilder _requestBuilder;
		private readonly EthUnsubscribeRequestBuilder _unsubscribeRequestBuilder;
		private readonly IWeb3 _web3Provider;

		private UniTaskCompletionSource<RpcStreamingResponseMessage> _taskCompletionSource;

		public ContractEventSubscriber(IWeb3 web3Provider, string wsUrl)
		{
			_web3Provider = web3Provider;
			_wsUrl = wsUrl;
			_subscribers = new Dictionary<string, IContractEventSubscription>();
			_requestBuilder = new EthLogsSubscriptionRequestBuilder();
			_unsubscribeRequestBuilder = new EthUnsubscribeRequestBuilder();
		}

		public async UniTask ListenForEvents()
		{
			_isCancellationRequested = false;
			this.SetBasicAuthenticationHeaderFromUri(new Uri(_wsUrl));

			_transport = new WebSocket(_wsUrl, RequestHeaders);

			_transport.OnOpen += OnSocketOpen;
			_transport.OnMessage += OnEventMessageReceived;
			_transport.OnClose += OnClose;
			_transport.OnError += OnError;

			Update();
			
			var connectTask = _transport.Connect();
			await connectTask;

			if (connectTask.IsFaulted)
			{
				Debug.LogError(connectTask.Exception);
			}
		}

		private async UniTaskVoid Update()
		{
			while (!_isCancellationRequested)
			{
				_transport.DispatchMessageQueue();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}

		public async UniTask<IContractEventSubscription> Subscribe<TEventType>(EventFilterData evFilter, string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new()
		{
			var filters = TransformFilters<TEventType>(evFilter, contractAddress);
			var subscriptionId = await CreateSubscription(filters);

			var eventSubscription = new ContractEventSubscription<TEventType>(subscriptionId, filters, handler);
			_subscribers.Add(subscriptionId, eventSubscription);

			return eventSubscription;
		}

		private async UniTask<string> CreateSubscription(NewFilterInput filters)
		{
			var rpcRequestJson = CreateRequest(filters);

			Debug.Log(rpcRequestJson);

			_taskCompletionSource = new UniTaskCompletionSource<RpcStreamingResponseMessage>();
			await _transport.SendText(rpcRequestJson);

			var connectionMessage = await _taskCompletionSource.Task;

			var subscriptionId = connectionMessage.GetStreamingResult<string>();

			return subscriptionId;
		}

		private string CreateRequest(NewFilterInput filterInput, string id = null)
		{
			var request = _requestBuilder.BuildRequest(filterInput, id);

			var reqMsg = new RpcRequestMessage(request.Id,
				request.Method,
				request.RawParameters);

			return JsonConvert.SerializeObject(reqMsg);
		}

		private NewFilterInput TransformFilters<TEventType>(EventFilterData evFilter, string contractAddress)
			where TEventType : IEventDTO, new()
		{
			var eventHandler = _web3Provider.Eth.GetEvent<TEventType>(contractAddress);
			var filters = EventFilterHelper.CreateEventFilters(eventHandler, evFilter);
			filters.Address = null;
			filters.FromBlock = null;
			filters.ToBlock = null;

			return filters;
		}

		public async void Unsubscribe(IContractEventSubscription subscription)
		{
			var rpcRequest = _unsubscribeRequestBuilder.BuildRequest(subscription.SubscriptionId);
			await _transport.SendText(JsonConvert.SerializeObject(rpcRequest));
		}

		private RpcStreamingResponseMessage DeserializeMessage(byte[] message)
		{
			var messageJson = Encoding.UTF8.GetString(message);
			return JsonConvert.DeserializeObject<RpcStreamingResponseMessage>(messageJson);
		}

		private void OnSocketOpen()
		{
			OnOpenHandler?.Invoke();
		}

		private void OnEventMessageReceived(byte[] rpcAnswer)
		{
			Debug.Log("----- OnMessageReceived -----");
			Debug.Log(Encoding.UTF8.GetString(rpcAnswer));
			var rpcMessage = DeserializeMessage(rpcAnswer);

			if (rpcMessage.Method == null)
			{
				_taskCompletionSource.TrySetResult(rpcMessage);
			}
			else
			{
				var subscriptionId = rpcMessage.Params?.Subscription;

				if (rpcMessage.Method == "eth_subscription" && subscriptionId != null &&
				    _subscribers.ContainsKey(subscriptionId))
				{
					_subscribers[subscriptionId].HandleMessage(rpcMessage);
				}	
			}
		}

		public void StopListen()
		{
			CloseConnection();
			_transport.Close();
			_subscribers.Clear();
		}

		private void CloseConnection()
		{
			_isCancellationRequested = true;
			
			_transport.OnOpen -= OnSocketOpen;
			_transport.OnMessage -= OnEventMessageReceived;
			_transport.OnClose -= OnClose;
			_transport.OnError -= OnError;	
		}

		private void RefreshConnection()
		{
			CloseConnection();
			ListenForEvents();
		}

		private void OnError(string errorMesssage)
		{
			Debug.Log("----- !!! Error !!! -----");
			RefreshConnection();
			OnErrorHandler?.Invoke(errorMesssage);
		}

		private void OnClose(WebSocketCloseCode code)
		{
			Debug.Log($"----- Socket closed ({code}) -----");
			if (code == WebSocketCloseCode.Abnormal)
			{
				RefreshConnection();
			}
			OnCloseHandler?.Invoke(code);
		}
	}
}