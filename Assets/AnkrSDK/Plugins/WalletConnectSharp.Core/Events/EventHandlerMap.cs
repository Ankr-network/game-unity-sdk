using System;
using System.Collections.Generic;

namespace AnkrSDK.WalletConnectSharp.Core.Events
{
    public class EventHandlerMap<TEventArgs>
    {
        private readonly Dictionary<string, EventHandler<TEventArgs>> _mapping =
            new Dictionary<string, EventHandler<TEventArgs>>();

        private readonly EventHandler<TEventArgs> _beforeEventExecuted;

        public EventHandlerMap(EventHandler<TEventArgs> callbackBeforeExecuted)
        {
            if (callbackBeforeExecuted == null)
            {
                callbackBeforeExecuted = CallbackBeforeExecuted;
            }

            _beforeEventExecuted = callbackBeforeExecuted;
        }

        private void CallbackBeforeExecuted(object sender, TEventArgs e)
        {
        }

        public EventHandler<TEventArgs> this[string topic]
        {
            get
            {
                if (!_mapping.ContainsKey(topic))
                {
                    _mapping.Add(topic, _beforeEventExecuted);
                }
                
                return _mapping[topic];
            }
            set
            {
                if (_mapping.ContainsKey(topic))
                {
                    _mapping.Remove(topic);
                }
                
                _mapping.Add(topic, value);
            }
        }

        public bool Contains(string topic)
        {
            return _mapping.ContainsKey(topic);
        }
    }
}