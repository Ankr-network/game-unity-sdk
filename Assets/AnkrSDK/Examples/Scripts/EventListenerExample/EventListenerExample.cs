using System;
using System.Numerics;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Data.ContractMessages.ERC721;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.DTO;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.UseCases;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.EventListenerExample
{
	public class EventListenerExample : UseCase
	{
		private IContract _erc20Contract;
		private EthHandler _eth;
		private ContractEventSubscriber _eventSubscriber;
		private IContractEventSubscription _subscription;

		private void Start()
		{
			var ankrSDK = AnkrSDKWrapper.GetSDKInstance(ERC20ContractInformation.HttpProviderURL);

			_eventSubscriber = ankrSDK.GetSubscriber(ERC20ContractInformation.WsProviderURL);
			_eventSubscriber.ListenForEvents();
			_eventSubscriber.OnOpenHandler += Subscribe;
		}

		public async void Subscribe()
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

		public async void Unsubscribe()
		{
			_eventSubscriber.Unsubscribe(_subscription);
		}
		

		private void OnDisable()
		{
			_eventSubscriber.StopListen();
		}
	}
}