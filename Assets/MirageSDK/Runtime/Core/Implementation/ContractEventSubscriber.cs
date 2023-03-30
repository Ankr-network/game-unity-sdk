using System;
using System.Collections.Generic;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using MirageSDK.Data;
using MirageSDK.WalletConnectSharp.Unity.Network.Client;
using MirageSDK.WalletConnectSharp.Unity.Network.Client.Data;
using MirageSDK.WalletConnectSharp.Unity.Network.Client.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Subscriptions;
using UnityEngine;

namespace MirageSDK.Core.Implementation
{
	public class ContractEventSubscriber : IClientRequestHeaderSupport, IContractEventSubscriber
	{
		private readonly EthLogsSubscriptionRequestBuilder _requestBuilder;
		private readonly Dictionary<string, IContractEventSubscription> _subscribers;
		private readonly EthUnsubscribeRequestBuilder _unsubscribeRequestBuilder;
		private readonly string _wsUrl;

		private bool _isCancellationRequested;
		private UniTaskCompletionSource<RpcStreamingResponseMessage> _taskCompletionSource;
		private IWebSocket _transport;
		private UniTaskCompletionSource _transportOpeningTaskCompletionSource;

		public ContractEventSubscriber(string wsUrl)
		{
			_wsUrl = wsUrl;
			_subscribers = new Dictionary<string, IContractEventSubscription>();
			_requestBuilder = new EthLogsSubscriptionRequestBuilder();
			_unsubscribeRequestBuilder = new EthUnsubscribeRequestBuilder();
		}

		public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

		public UniTask SocketOpeningTask
		{
			get
			{
				if (_transportOpeningTaskCompletionSource == null)
				{
					Debug.LogError("Listen for events not started");
					return UniTask.CompletedTask;
				}

				return _transportOpeningTaskCompletionSource.Task;
			}
		}

		public event Action OnOpenHandler;
		public event Action<string> OnErrorHandler;
		public event Action<WebSocketCloseCode> OnCloseHandler;

		public async UniTask ListenForEvents()
		{
			_transportOpeningTaskCompletionSource = new UniTaskCompletionSource();
			_isCancellationRequested = false;
			this.SetBasicAuthenticationHeaderFromUri(new Uri(_wsUrl));

			_transport = WebSocketFactory.CreateInstance(_wsUrl);

			_transport.OnOpen += OnTransportOpen;
			_transport.OnMessage += OnEventMessageReceived;
			_transport.OnClose += OnClose;
			_transport.OnError += OnError;

			Update().Forget();


			var connectTask = _transport.Connect().AsTask();
			await connectTask;


			if (connectTask.IsFaulted)
			{
				Debug.LogError("ContractEventSubscribed " + connectTask.Exception);
			}

			Debug.Log("Listen for events socket connected");
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

		private async UniTaskVoid Update()
		{
			while (!_isCancellationRequested)
			{
				_transport.DispatchMessageQueue();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
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

		private void OnTransportOpen()
		{
			_transportOpeningTaskCompletionSource?.TrySetResult();
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

			_transport.OnOpen -= OnTransportOpen;
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