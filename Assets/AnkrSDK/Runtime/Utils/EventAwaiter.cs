using System;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Utils
{
	public class EventAwaiter<T> where T : EventDTOBase, new()
	{
		private readonly string _contractAddress;
		private EventFilterRequest<T> _filter;
		private readonly IContractEventSubscriber _eventSubscriber;
		private UniTaskCompletionSource<T> _receiveEventCompletionSource;
		private T _resultEventDto;
		private IContractEventSubscription _subscription;
		
		public UniTask<T> ReceiveEventTask
		{
			get
			{
				if (_resultEventDto != null)
				{
					return UniTask.FromResult(_resultEventDto);
				}
				
				if (_receiveEventCompletionSource == null)
				{
					throw new InvalidOperationException("Waiting for event was not started");
				}

				return _receiveEventCompletionSource.Task;
			}
		}

		public EventAwaiter(string contractAddress, string wsProviderURL)
		{
			_contractAddress = contractAddress;
			_eventSubscriber = new ContractEventSubscriber(wsProviderURL);
			
		}

		public async UniTask StartWaiting(EventFilterRequest<T> filtersRequest)
		{
			_receiveEventCompletionSource = new UniTaskCompletionSource<T>();
			_eventSubscriber.ListenForEvents().Forget();
			await _eventSubscriber.SocketOpeningTask;
			
			try
			{
				_subscription = await _eventSubscriber.Subscribe(
					filtersRequest,
					_contractAddress,
					Callback
				);
			}
			catch (Exception e)
			{
				_receiveEventCompletionSource.TrySetException(e);
			}
			
		}

		private void Callback(T eventDto)
		{
			try
			{
				_resultEventDto = eventDto;
				_eventSubscriber.Unsubscribe(_subscription.SubscriptionId);
			}
			catch (Exception e)
			{
				_receiveEventCompletionSource.TrySetException(e);
				return;
			}
			
			_receiveEventCompletionSource.TrySetResult(eventDto);
		}
	}
}