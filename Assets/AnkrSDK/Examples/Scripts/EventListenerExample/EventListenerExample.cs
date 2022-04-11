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

		private async void Start()
		{
			var ankrSDK = AnkrSDKWrapper.GetSDKInstance(ERC20ContractInformation.HttpProviderURL);

			_eventSubscriber = ankrSDK.GetSubscriber(ERC20ContractInformation.WsProviderURL);
			_eventSubscriber.OnOpenHandler += SendMint;
			await _eventSubscriber.ListenForEvents();
			SendMint();
		}

		public async void SendMint()
		{
			var filters = new EventFilterData
			{
				fromBlock = BlockParameter.CreateLatest(),
				toBlock = BlockParameter.CreateLatest()
			};

			var subscription = await _eventSubscriber.Subscribe(
				filters,
				ERC20ContractInformation.ContractAddress, 
				(TransferEventDTO t) => Debug.Log(JsonConvert.SerializeObject(t))
			);
		}

		private void OnDisable()
		{
			_eventSubscriber.StopListen();
		}
	}
}