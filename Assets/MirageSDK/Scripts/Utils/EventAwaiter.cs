using System;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.Utils
{
	public class EventAwaiter<TEventDto> where TEventDto : EventDTOBase, new()
	{
		private readonly string _contractAddress;
		private readonly IContractEventSubscriber _eventSubscriber;
		private EventFilterRequest<TEventDto> _filter;
		private UniTaskCompletionSource<TEventDto> _receiveEventCompletionSource;
		private TEventDto _resultEventDto;

		private IContractEventSubscription _subscription;

		public EventAwaiter(string contractAddress, string wsProviderURL)
		{
			_contractAddress = contractAddress;
			_eventSubscriber = new ContractEventSubscriber(wsProviderURL);
		}

		public UniTask<TEventDto> ReceiveEventTask
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

		public async UniTask StartWaiting(EventFilterRequest<TEventDto> filtersRequest)
		{
			_receiveEventCompletionSource = new UniTaskCompletionSource<TEventDto>();
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
				Debug.LogError("EventAwaiter exception: " + e.Message);
				_receiveEventCompletionSource.TrySetException(e);
			}
		}

		private void Callback(TEventDto eventDto)
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