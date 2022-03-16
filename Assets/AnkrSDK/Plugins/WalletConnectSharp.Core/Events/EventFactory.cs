using System;
using System.Collections.Generic;

namespace AnkrSDK.WalletConnectSharp.Core.Events
{
    public class EventFactory
    {
        private static EventFactory _instance;
        
        private readonly Dictionary<Type, IEventProvider> _eventProviders = new Dictionary<Type, IEventProvider>();

        public static EventFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventFactory();
                }
                return _instance;
            }
        }

        public void Register<T>(IEventProvider provider)
        {
            var t = typeof(T);

            if (_eventProviders.ContainsKey(t))
            {
                return;
            }

            _eventProviders.Add(t, provider);
        }

        public IEventProvider ProviderFor<T>()
        {
            var t = typeof(T);
            return _eventProviders.ContainsKey(t) ? _eventProviders[t] : null;
        }
    }
}