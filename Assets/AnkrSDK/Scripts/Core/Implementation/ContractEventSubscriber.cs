using System;
using System.Collections.Generic;
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
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{	
	public class ContractEventSubscriber : IClientRequestHeaderSupport
	{
		private readonly string _wsUrl;
		private readonly Dictionary<string, IContractEventSubscription> _subscribers;
		private readonly EthLogsSubscriptionRequestBuilder _requestBuilder;
		private readonly EthUnsubscribeRequestBuilder _unsubscribeRequestBuilder;
		private readonly IWeb3 _web3Provider;
		
		private WebSocket _transport;
		private bool _isCancellationRequested;
		private UniTaskCompletionSource<RpcStreamingResponseMessage> _taskCompletionSource;
		
		public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
		
		public event Action OnOpenHandler;
		public event Action<string> OnErrorHandler;
		public event Action<WebSocketCloseCode> OnCloseHandler;
		
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

			Update().Forget();
			
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
			var filters = EventFilterHelper.CreateEventFilters<TEventType>(contractAddress, evFilter);
			var subscriptionId = await CreateSubscription(filters);

			var eventSubscription = new ContractEventSubscription<TEventType>(subscriptionId, handler);
			_subscribers.Add(subscriptionId, eventSubscription);

			return eventSubscription;
		}
		
		public async UniTask<IContractEventSubscription> Subscribe<TEventType>(EventFilterRequest<TEventType> evFilter, string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new()
		{
			var filters = EventFilterHelper.CreateEventFilters(contractAddress, evFilter);
			var subscriptionId = await CreateSubscription(filters);

			var eventSubscription = new ContractEventSubscription<TEventType>(subscriptionId, handler);
			_subscribers.Add(subscriptionId, eventSubscription);

			return eventSubscription;
		}

		private async UniTask<string> CreateSubscription(NewFilterInput filters)
		{
			var rpcRequestJson = CreateRequest(filters);

			_taskCompletionSource = new UniTaskCompletionSource<RpcStreamingResponseMessage>();
			await _transport.SendText(rpcRequestJson);

			var connectionMessage = await _taskCompletionSource.Task;

			var subscriptionId = connectionMessage.GetStreamingResult<string>();

			return subscriptionId;
		}

		private string CreateRequest(NewFilterInput filterInput, string id = null)
		{
			var request = _requestBuilder.BuildRequest(filterInput, id);

			return EventSubscriberHelper.RpcRequestToString(request);
		}

		public async UniTaskVoid Unsubscribe(IContractEventSubscription subscription)
		{
			_subscribers.Remove(subscription.SubscriptionId);
			var rpcRequest = _unsubscribeRequestBuilder.BuildRequest(subscription.SubscriptionId);
			var requestMessage = EventSubscriberHelper.RpcRequestToString(rpcRequest);
			await _transport.SendText(requestMessage);
		}

		private void OnSocketOpen()
		{
			OnOpenHandler?.Invoke();
		}

		private void OnEventMessageReceived(byte[] rpcAnswer)
		{
			var rpcMessage = EventSubscriberHelper.DeserializeMessage(rpcAnswer);

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
			_subscribers.Clear();
			_transport.Close();
		}

		private void CloseConnection()
		{
			_isCancellationRequested = true;
			
			_transport.OnOpen -= OnSocketOpen;
			_transport.OnMessage -= OnEventMessageReceived;
			_transport.OnClose -= OnClose;
			_transport.OnError -= OnError;	
		}

		private void OnError(string errorMesssage)
		{
			StopListen();
			OnErrorHandler?.Invoke(errorMesssage);
		}

		private void OnClose(WebSocketCloseCode code)
		{
			StopListen();
			OnCloseHandler?.Invoke(code);
		}
	}
}