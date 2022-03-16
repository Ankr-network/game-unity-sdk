using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events
{
	public class EventManager<T, TEventArgs> : IEventProvider where TEventArgs : IEvent<T>, new()
	{
		private static EventManager<T, TEventArgs> _instance;

		public readonly EventHandlerMap<TEventArgs> EventTriggers;

		public static EventManager<T, TEventArgs> Instance =>
			_instance ?? (_instance = new EventManager<T, TEventArgs>());

		private EventManager()
		{
			EventTriggers = new EventHandlerMap<TEventArgs>(CallbackBeforeExecuted);

			EventFactory.Instance.Register<T>(this);
		}

		private void CallbackBeforeExecuted(object sender, TEventArgs e)
		{
		}

		public void PropagateEvent(string topic, string responseJson)
		{
			if (!EventTriggers.Contains(topic))
			{
				return;
			}

			var eventTrigger = EventTriggers[topic];

			if (eventTrigger == null)
			{
				return;
			}

			var response = JsonConvert.DeserializeObject<T>(responseJson);
			var eventArgs = new TEventArgs();
			eventArgs.SetData(response);
			eventTrigger(this, eventArgs);
		}
	}
}