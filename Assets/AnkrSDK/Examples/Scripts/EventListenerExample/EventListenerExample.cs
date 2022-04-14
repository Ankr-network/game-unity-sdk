using AnkrSDK.Core.Data;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.DTO;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.UseCases;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AnkrSDK.EventListenerExample
{
	public class EventListenerExample : UseCase
	{
		private IContract _erc20Contract;
		private EthHandler _eth;
		private ContractEventSubscriber _eventSubscriber;
		private IContractEventSubscription _subscription;
		
		public override void ActivateUseCase()
		{
			base.ActivateUseCase();
			
			var ankrSDK = AnkrSDKWrapper.GetSDKInstance(ERC20ContractInformation.HttpProviderURL);

			_eventSubscriber = ankrSDK.GetSubscriber(ERC20ContractInformation.WsProviderURL);
			_eventSubscriber.ListenForEvents().Forget();
			_eventSubscriber.OnOpenHandler += UniTask.Action(Subscribe);
		}

		public async UniTaskVoid Subscribe()
		{
			var filters = new EventFilterData
			{
				fromBlock = BlockParameter.CreateLatest(),
				toBlock = BlockParameter.CreateLatest()
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

		public void DeActivateUseCase()
		{
			base.DeActivateUseCase();
			_eventSubscriber.StopListen();
		}
	}
}