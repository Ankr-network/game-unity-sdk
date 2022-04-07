using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
	public class ContractEventSubscriber : IClientRequestHeaderSupport
	{
		public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
		private WebSocket _transport;

		private readonly string _wsUrl;
		private readonly Dictionary<string, IContractEventSubscription> _subscribers;
		private readonly EthLogsSubscriptionRequestBuilder _requestBuilder;
		private readonly IWeb3 _web3Provider;

		private UniTaskCompletionSource<RpcStreamingResponseMessage> _taskCompletionSource;

		public ContractEventSubscriber(IWeb3 web3Provider, string wsUrl)
		{
			_web3Provider = web3Provider;
			_wsUrl = wsUrl;
			_subscribers = new Dictionary<string, IContractEventSubscription>();
			_requestBuilder = new EthLogsSubscriptionRequestBuilder();
		}

		public async UniTask ListenForEvents()
		{
			this.SetBasicAuthenticationHeaderFromUri(new Uri(_wsUrl));

			_transport = new WebSocket(_wsUrl, RequestHeaders);

			_transport.OnOpen += OnSocketOpen;
			_transport.OnMessage += OnTransportMessageReceived;
			_transport.OnClose += OnClose;
			_transport.OnError += OnError;

			var connectTask = _transport.Connect();
			await connectTask;

			if (connectTask.IsFaulted)
			{
				Debug.LogError(connectTask.Exception);
			}
		}

		private async UniTaskVoid Update(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				_transport.DispatchMessageQueue();

				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}

		public async UniTask<string> Subscribe<TEventType>(EventFilterData evFilter, string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new()
		{
			var subscriptionId = await CreateSubscription<TEventType>(evFilter, contractAddress);

			var eventSubscription = new ContractEventSubscription<TEventType>(handler);
			_subscribers.Add(subscriptionId, eventSubscription);

			return subscriptionId;
		}

		private async UniTask<string> CreateSubscription<TEventType>(EventFilterData evFilter, string contractAddress)
			where TEventType : IEventDTO, new()
		{
			var filters = TransformFilters<TEventType>(evFilter, contractAddress);

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

		public void Unsubscribe(string subscriptionId)
		{
			_subscribers.Remove(subscriptionId);
		}

		private RpcStreamingResponseMessage DeserializeMessage(byte[] message)
		{
			var messageJson = Encoding.UTF8.GetString(message);
			return JsonConvert.DeserializeObject<RpcStreamingResponseMessage>(messageJson);
		}

		private void OnSocketOpen()
		{
			Debug.Log("----- Socket opened -----");
		}

		private void OnTransportMessageReceived(byte[] rpcAnswer)
		{
			var rpcMessage = DeserializeMessage(rpcAnswer);

			_transport.OnMessage -= OnTransportMessageReceived;
			_taskCompletionSource.TrySetResult(rpcMessage);

			_transport.OnMessage += OnTransportMessageReceived;
		}

		private void OnEventMessageReceived(byte[] rpcAnswer)
		{
			Debug.Log("----- OnMessageReceived -----");
			Debug.Log(Encoding.UTF8.GetString(rpcAnswer));
			var rpcMessage = DeserializeMessage(rpcAnswer);

			_taskCompletionSource.TrySetResult(rpcMessage);
			var subscriptionId = rpcMessage?.Params?.Subscription;

			if (rpcMessage.Method == "eth_subscription" && subscriptionId != null &&
			    _subscribers.ContainsKey(subscriptionId))
			{
				_subscribers[subscriptionId].HandleMessage(rpcMessage);
			}
		}

		private void OnError(string errorMesssage)
		{
			Debug.Log("----- !!! Error !!! -----");
		}

		private void OnClose(WebSocketCloseCode code)
		{
			Debug.Log($"----- Socket closed ({code}) -----");
		}
	}
}