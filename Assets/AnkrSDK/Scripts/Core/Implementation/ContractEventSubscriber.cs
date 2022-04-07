using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Examples.DTO;
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
		private delegate void ContractEventHandler(RpcStreamingResponseMessage data);
		
		public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
		private string _wsUrl;
		private WebSocket _transport;
		private event ContractEventHandler OnMessage;
		private Dictionary<string, IContractEventSubscription<EventDTOBase>> _subscribers;
		private EthLogsSubscriptionRequestBuilder _requestBuilder;
		private readonly IWeb3 _web3Provider;

		public ContractEventSubscriber(IWeb3 web3Provider, string wsUrl)
		{
			_web3Provider = web3Provider;
			_wsUrl = wsUrl;
			_subscribers = new Dictionary<string, IContractEventSubscription<EventDTOBase>>();
			_requestBuilder = new EthLogsSubscriptionRequestBuilder();
		}

		public async Task Connect()
		{
			this.SetBasicAuthenticationHeaderFromUri(new Uri(_wsUrl));

			_transport = new WebSocket(_wsUrl, RequestHeaders);

			_transport.OnOpen += OnSocketOpen;
			_transport.OnMessage += OnMessageReceived;
			_transport.OnClose += OnClose;
			_transport.OnError += OnError;

			var connectTask =  _transport.Connect();
			await connectTask;

			if (connectTask.IsFaulted)
			{
				HandleError(connectTask.Exception);
			}
		}
		
		public void Update()
		{
			_transport.DispatchMessageQueue();
		}

		private void HandleError(Exception e)
		{
			Debug.LogError(e);
		}

		public async Task<string> Subscribe<TEventType>(EventFilterData evFilter, string contractAddress, Action<TEventType> handler) where TEventType : EventDTOBase
		{
			var subscribtionId = await Subscribe(evFilter, contractAddress);
			
			var subscriprion = new ContractEventSubscription<TEventType>(handler);
			_subscribers.Add(subscribtionId, subscriprion);
			
			return subscribtionId;
		}

		public async Task<string> Subscribe(EventFilterData evFilter, string contractAddress)
		{
			var filters = TransformFilters(evFilter, contractAddress);
			
			var rpcRequestJson = CreateRequest(filters);
			
			Debug.Log(rpcRequestJson);
			
			var isSubscriptionMessage = false;
			RpcStreamingResponseMessage message = null;

			ContractEventHandler messageHandler = rpcAnswer =>
			{
				message = rpcAnswer;
				if (message.Method == null)
				{
					isSubscriptionMessage = true;
				}
			};

			OnMessage += messageHandler;

			await _transport.SendText(rpcRequestJson);

			UniTask.WaitUntil(() => isSubscriptionMessage);

			OnMessage -= messageHandler;

			var subscriptionId = message.GetStreamingResult<string>();

			return subscriptionId;
		}

		public string CreateRequest(NewFilterInput filterInput, object id = null)
		{
			var request = _requestBuilder.BuildRequest(filterInput, id);
			
			var reqMsg = new RpcRequestMessage(request.Id,
				request.Method,
				request.RawParameters);
			
			return JsonConvert.SerializeObject(reqMsg);
		}

		public NewFilterInput TransformFilters(EventFilterData evFilter, string contractAddress) 
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
			// Send message to unsubscribe

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

		private void OnMessageReceived(byte[] rpcAnswer)
		{
			Debug.Log("----- OnMessageReceived -----");
			Debug.Log(Encoding.UTF8.GetString(rpcAnswer));
			var rpcMessage = DeserializeMessage(rpcAnswer);
			OnMessage?.Invoke(rpcMessage);
			var subscriptionId = rpcMessage?.Params?.Subscription;

			if (rpcMessage.Method == "eth_subscription" && subscriptionId != null && _subscribers.ContainsKey(subscriptionId))
			{
				_subscribers[subscriptionId].Invoke(rpcMessage.GetResult<>());
				
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