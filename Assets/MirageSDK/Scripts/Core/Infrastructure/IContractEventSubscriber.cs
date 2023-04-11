using System;
using MirageSDK.Data;
using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Unity.Network.Client.Data;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MirageSDK.Core.Infrastructure
{
	public interface IContractEventSubscriber
	{
		event Action OnOpenHandler;
		event Action<string> OnErrorHandler;
		event Action<WebSocketCloseCode> OnCloseHandler;
		
		UniTask SocketOpeningTask { get; }

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