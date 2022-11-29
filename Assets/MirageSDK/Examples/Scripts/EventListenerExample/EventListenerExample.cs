using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.DTO;
using MirageSDK.ERC20Example;
using MirageSDK.Provider;
using MirageSDK.UseCases;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.EventListenerExample
{
	/// <summary>
	/// Your usual flow will be this:
	/// 	Get the subscriber instance with provided Endpoint
	///			_eventSubscriber = MirageSDK.CreateSubscriber(ERC20ContractInformation.WsProviderURL);
	///		Manually connect socket
	///			_eventSubscriber.ListenForEvents().Forget();
	///		Subscribe custom handler for events
	///			_eventSubscriber.OnOpenHandler += UniTask.Action(SubscribeWithTopics);
	///		Unsubscribe
	///			_eventSubscriber.Unsubscribe(_subscription).Forget();
	///		Manually stop listen
	///			_eventSubscriber.StopListen();
	/// 	</summary>
	public class EventListenerExample : UseCase
	{
		[SerializeField] private ContractInformationSO _contractInformationSO;
		private IContractEventSubscriber _eventSubscriber;
		private IContractEventSubscription _subscription;
		private IEthHandler _eth;
		
		public override void ActivateUseCase()
		{
			base.ActivateUseCase();
			

			var MirageSDK = MirageSDKFactory.GetMirageSDKInstance(_contractInformationSO.HttpProviderURL);
			_eth = MirageSDK.Eth;

			_eventSubscriber = MirageSDK.CreateSubscriber(_contractInformationSO.WsProviderURL);
			_eventSubscriber.ListenForEvents().Forget();
			_eventSubscriber.OnOpenHandler += UniTask.Action(SubscribeWithRequest);
		}

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

		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_eventSubscriber.StopListen();
		}
	}
}