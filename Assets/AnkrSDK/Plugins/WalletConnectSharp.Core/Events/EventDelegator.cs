using System;
using System.Collections.Generic;
using AnkrSDK.WalletConnectSharp.Core.Events.Model;
using AnkrSDK.WalletConnectSharp.Core.Models;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events
{
	public class EventDelegator : IDisposable
	{
		private Dictionary<string, List<IEventProvider>> Listeners = new Dictionary<string, List<IEventProvider>>();

		public void ListenForGenericResponse<T>(object id, EventHandler<GenericEvent<T>> callback)
		{
			ListenFor("response:" + id, callback);
		}

		public void ListenForResponse<T>(object id, EventHandler<JsonRpcResponseEvent<T>> callback)
			where T : JsonRpcResponse
		{
			ListenFor("response:" + id, callback);
		}

		public void ListenFor<T>(string eventId, EventHandler<GenericEvent<T>> callback)
		{
			EventManager<T, GenericEvent<T>>.Instance.EventTriggers[eventId] += callback;

			SubscribeProvider(eventId, EventFactory.Instance.ProviderFor<T>());
		}

		public void ListenFor<T>(string eventId, EventHandler<JsonRpcResponseEvent<T>> callback)
			where T : JsonRpcResponse
		{
			EventManager<T, JsonRpcResponseEvent<T>>.Instance.EventTriggers[eventId] += callback;

			SubscribeProvider(eventId, EventFactory.Instance.ProviderFor<T>());
		}

		public void ListenFor<T>(string eventId, EventHandler<JsonRpcRequestEvent<T>> callback) where T : JsonRpcRequest
		{
			EventManager<T, JsonRpcRequestEvent<T>>.Instance.EventTriggers[eventId] += callback;

			SubscribeProvider(eventId, EventFactory.Instance.ProviderFor<T>());
		}

		public void UnsubscribeProvider(string eventId)
		{
			Listeners.Remove(eventId);
		}

		private void SubscribeProvider(string eventId, IEventProvider provider)
		{
			List<IEventProvider> listProvider;
			if (!Listeners.ContainsKey(eventId))
			{
				listProvider = new List<IEventProvider>();
				Listeners.Add(eventId, listProvider);
			}
			else
			{
				listProvider = Listeners[eventId];
			}

			listProvider.Add(provider);
		}

		public bool Trigger<T>(string topic, T obj)
		{
			return Trigger(topic, JsonConvert.SerializeObject(obj));
		}

		public bool Trigger(string topic, string json)
		{
			if (!Listeners.ContainsKey(topic))
			{
				return false;
			}

			var providerList = Listeners[topic];

			foreach (var provider in providerList)
			{
				provider.PropagateEvent(topic, json);
			}

			return providerList.Count > 0;

		}

		public void Dispose()
		{
			Clear();
		}

		public void Clear()
		{
			Listeners.Clear();
		}
	}
}