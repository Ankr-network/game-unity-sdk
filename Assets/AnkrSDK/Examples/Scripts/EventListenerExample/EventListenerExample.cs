using AnkrSDK.Core.Data;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.DTO;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.UseCases;
using Cysharp.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
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
	public class EventListenerExample : UseCase
	{
		private ContractEventSubscriber _eventSubscriber;
		private IContractEventSubscription _subscription;
		
		public override void ActivateUseCase()
		{
			base.ActivateUseCase();
			
			var ankrSDK = AnkrSDKWrapper.GetSDKInstance(ERC20ContractInformation.HttpProviderURL);

			_eventSubscriber = ankrSDK.CreateSubscriber(ERC20ContractInformation.WsProviderURL);
			_eventSubscriber.ListenForEvents().Forget();
			_eventSubscriber.OnOpenHandler += UniTask.Action(SubscribeWithTopics);
		}

		// If you know topic position then you can use EventFilterData
		public async UniTaskVoid SubscribeWithTopics()
		{
			var filters = new EventFilterData
			{
				fromBlock = BlockParameter.CreateLatest(),
				toBlock = BlockParameter.CreateLatest(),
				filterTopic2 = new object[] { EthHandler.DefaultAccount }
			};

			_subscription = await _eventSubscriber.Subscribe(
				filters,
				ERC20ContractInformation.ContractAddress, 
				(TransferEventDTO t) => ReceiveEvent(t)
			);
		}

		private void ReceiveEvent(TransferEventDTO contractEvent)
		{
			Debug.Log($"{contractEvent.From} - {contractEvent.To} - {contractEvent.Value}");
		}

		public void Unsubscribe()
		{
			_eventSubscriber.Unsubscribe(_subscription).Forget();
		}

		public override void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_eventSubscriber.StopListen();
		}
	}
}