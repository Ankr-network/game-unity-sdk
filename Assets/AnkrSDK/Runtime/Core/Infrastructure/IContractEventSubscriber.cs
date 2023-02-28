using System;
using AnkrSDK.Data;
using AnkrSDK.WalletConnectSharp.Unity.Network.Client.Data;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IContractEventSubscriber
	{
		event Action OnOpenHandler;
		event Action<string> OnErrorHandler;
		event Action<WebSocketCloseCode> OnCloseHandler;

		UniTask ListenForEvents();
		void StopListen();

		UniTask<IContractEventSubscription> Subscribe<TEventType>(EventFilterData evFilter, string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new();

		UniTask<IContractEventSubscription> Subscribe<TEventType>(EventFilterRequest<TEventType> evFilter,
			string contractAddress,
			Action<TEventType> handler) where TEventType : IEventDTO, new();

		UniTask Unsubscribe(string subscriptionId);
	}
}