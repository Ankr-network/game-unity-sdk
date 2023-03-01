using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Data;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Subscriptions;
using System.Collections.Generic;
using System;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure;
using UnityEngine;

namespace AnkrSDK.Core.Implementation
{
	internal class ContractEventSubscriber : IClientRequestHeaderSupport, IContractEventSubscriber
	{
		private readonly string _wsUrl;
		private readonly Dictionary<string, IContractEventSubscription> _subscribers;
		private readonly EthLogsSubscriptionRequestBuilder _requestBuilder;
		private readonly EthUnsubscribeRequestBuilder _unsubscribeRequestBuilder;

		private bool _isCancellationRequested;
		private IWebSocket _transport;
		private UniTaskCompletionSource<RpcStreamingResponseMessage> _taskCompletionSource;

		public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

		public event Action OnOpenHandler;
		public event Action<string> OnErrorHandler;
		public event Action<WebSocketCloseCode> OnCloseHandler;

		public ContractEventSubscriber(string wsUrl)
		{
			_wsUrl = wsUrl;
			_subscribers = new Dictionary<string, IContractEventSubscription>();
			_requestBuilder = new EthLogsSubscriptionRequestBuilder();
			_unsubscribeRequestBuilder = new EthUnsubscribeRequestBuilder();
		}

		public async UniTask ListenForEvents()
		{
			_isCancellationRequested = false;
			this.SetBasicAuthenticationHeaderFromUri(new Uri(_wsUrl));

			_transport = WebSocketFactory.CreateInstance(_wsUrl);

			_transport.OnOpen += OnSocketOpen;
			_transport.OnMessage += OnEventMessageReceived;
			_transport.OnClose += OnClose;
			_transport.OnError += OnError;

			Update().Forget();

			var connectTask = _transport.Connect().AsTask();
			await connectTask;

			if (connectTask.IsFaulted)
			{
				Debug.LogError(connectTask.Exception);
			}

			Debug.Log("Listen for events socket connected");
		}

		private async UniTaskVoid Update()
		{
			while (!_isCancellationRequested)
			{
				_transport.DispatchMessageQueue();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}

		public async UniTask<IContractEventSubscription> Subscribe<TEventType>(EventFilterData evFilter,
			string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new()
		{
			var filters = EventFilterHelper.CreateEventFilters<TEventType>(contractAddress, evFilter);
			var subscriptionId = await CreateSubscription(filters);

			var eventSubscription = new ContractEventSubscription<TEventType>(subscriptionId, handler);
			_subscribers.Add(subscriptionId, eventSubscription);

			return eventSubscription;
		}

		public async UniTask<IContractEventSubscription> Subscribe<TEventType>(EventFilterRequest<TEventType> evFilter,
			string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new()
		{
			var filters = EventFilterHelper.CreateEventFilters(contractAddress, evFilter);
			var subscriptionId = await CreateSubscription(filters);

			var eventSubscription = new ContractEventSubscription<TEventType>(subscriptionId, handler);
			_subscribers.Add(subscriptionId, eventSubscription);

			return eventSubscription;
		}

		public UniTask Unsubscribe(string subscriptionId)
		{
			_subscribers.Remove(subscriptionId);
			var rpcRequest = _unsubscribeRequestBuilder.BuildRequest(subscriptionId);
			var requestMessage = EventSubscriberHelper.RpcRequestToString(rpcRequest);
			return _transport.SendText(requestMessage);
		}

		public void StopListen()
		{
			CloseConnection();
			_subscribers.Clear();
			_transport.Close();
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

				const string ethSubscriptionMethod = "eth_subscription";

				if (rpcMessage.Method == ethSubscriptionMethod && subscriptionId != null &&
				    _subscribers.ContainsKey(subscriptionId))
				{
					_subscribers[subscriptionId].HandleMessage(rpcMessage);
				}
			}
		}

		private void CloseConnection()
		{
			_isCancellationRequested = true;

			_transport.OnOpen -= OnSocketOpen;
			_transport.OnMessage -= OnEventMessageReceived;
			_transport.OnClose -= OnClose;
			_transport.OnError -= OnError;
		}

		private void OnError(string errorMessage)
		{
			StopListen();
			OnErrorHandler?.Invoke(errorMessage);
		}

		private void OnClose(WebSocketCloseCode code)
		{
			StopListen();
			OnCloseHandler?.Invoke(code);
		}
	}
}