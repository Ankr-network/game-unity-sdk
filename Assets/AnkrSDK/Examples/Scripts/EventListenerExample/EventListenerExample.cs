using AnkrSDK.Base;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.DTO;
using AnkrSDK.Provider;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.EventListenerExample
{
	/// <summary>
	/// Your usual flow will be this:
	/// 	Get the subscriber instance with provided Endpoint
	///			_eventSubscriber = ankrSDK.CreateSubscriber(ERC20ContractInformation.WsProviderURL);
	///		Manually connect socket
	///			_eventSubscriber.ListenForEvents().Forget();
	///		Subscribe custom handler for events
	///			_eventSubscriber.OnOpenHandler += UniTask.Action(SubscribeWithTopics);
	///		Unsubscribe
	///			_eventSubscriber.Unsubscribe(_subscription).Forget();
	///		Manually stop listen
	///			_eventSubscriber.StopListen();
	/// 	</summary>
	public class EventListenerExample : UseCaseBodyUI
	{
		[SerializeField] private ContractInformationSO _contractInformationSO;
		private IContractEventSubscriber _eventSubscriber;
		private IContractEventSubscription _subscription;
		private IEthHandler _eth;

		// If you know topic position then you can use EventFilterData
		public async UniTaskVoid SubscribeWithTopics()
		{
			var filters = new EventFilterData
			{
				FilterTopic2 = new object[] { await _eth.GetDefaultAccount() }
			};

			_subscription = await _eventSubscriber.Subscribe(
				filters,
				_contractInformationSO.ContractAddress, 
				(TransferEventDTO t) => ReceiveEvent(t)
			);
		}
		
		// If you know only topic name then you can use EventFilterRequest
		public async UniTaskVoid SubscribeWithRequest()
		{
			var filtersRequest = new EventFilterRequest<TransferEventDTO>();
			filtersRequest.AddTopic("To", await _eth.GetDefaultAccount());

			_subscription = await _eventSubscriber.Subscribe(
				filtersRequest,
				_contractInformationSO.ContractAddress, 
				ReceiveEvent
			);
		}

		private void ReceiveEvent(TransferEventDTO contractEvent)
		{
			Debug.Log($"{contractEvent.From} - {contractEvent.To} - {contractEvent.Value}");
		}

		public void Unsubscribe()
		{
			_eventSubscriber.Unsubscribe(_subscription.SubscriptionId).Forget();
		}

		public override void SetUseCaseBodyActive(bool active)
		{
			base.SetUseCaseBodyActive(active);

			if (active)
			{
				var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(_contractInformationSO.HttpProviderURL);
				_eth = ankrSDK.Eth;

				_eventSubscriber = ankrSDK.CreateSubscriber(_contractInformationSO.WsProviderURL);
				_eventSubscriber.ListenForEvents().Forget();
				_eventSubscriber.OnOpenHandler += UniTask.Action(SubscribeWithRequest);
			}
			else
			{
				_eventSubscriber.StopListen();
			}
		}
	}
}