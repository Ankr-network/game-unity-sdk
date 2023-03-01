using System;
using System.Collections.Generic;
using AnkrSDK.WalletConnectSharp.Core.Events.Model;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events
{
	public class EventDelegator : IDisposable
	{
		private readonly Dictionary<string, List<IEventProvider>> _listeners = new Dictionary<string, List<IEventProvider>>();

		public void ListenForGenericResponse<T>(object id, EventHandler<GenericEvent<T>> callback)
		{
			ListenForGeneric("response:" + id, callback);
		}

		public void ListenForResponse<T>(object id, EventHandler<JsonRpcResponseEvent<T>> callback)
		{
			ListenFor("response:" + id, callback);
		}

		public void ListenForGeneric<T>(string eventId, EventHandler<GenericEvent<T>> callback)
		{
			EventManager<T, GenericEvent<T>>.Instance.EventTriggers[eventId] += callback;

			SubscribeProvider(eventId, EventFactory.Instance.ProviderFor<T>());
		}

		private void ListenFor<T>(string eventId, EventHandler<JsonRpcResponseEvent<T>> callback)
		{
			EventManager<T, JsonRpcResponseEvent<T>>.Instance.EventTriggers[eventId] += callback;

			SubscribeProvider(eventId, EventFactory.Instance.ProviderFor<T>());
		}

		public void UnsubscribeProvider(string eventId)
		{
			_listeners.Remove(eventId);
		}

		private void SubscribeProvider(string eventId, IEventProvider provider)
		{
			List<IEventProvider> listProvider;
			if (!_listeners.ContainsKey(eventId))
			{
				listProvider = new List<IEventProvider>();
				_listeners.Add(eventId, listProvider);
			}
			else
			{
				listProvider = _listeners[eventId];
			}

			listProvider.Add(provider);
		}

		public bool Trigger<T>(string topic, T obj)
		{
			return Trigger(topic, JsonConvert.SerializeObject(obj));
		}

		public bool Trigger(string topic, string json)
		{
			if (!_listeners.ContainsKey(topic))
			{
				return false;
			}

			var providerList = _listeners[topic];

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
			_listeners.Clear();
		}
	}
}