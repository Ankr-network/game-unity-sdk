using System;
using System.Numerics;
using System.Threading.Tasks;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Data.ContractMessages.ERC721;
using AnkrSDK.Core.Events.Implementation;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Examples.DTO;
using AnkrSDK.UseCases;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AnkrSDK.Examples.ERC20Example
{
	public class ERC20Example : UseCase
	{
		private const string MintMethodName = "mint";
		private IContract _erc20Contract;
		private EthHandler _eth;
		private ContractEventSubscriber<> _eventSubscriber;

		private async void Start()
		{
			var ankrSDK = AnkrSDKWrapper.GetSDKInstance(ERC20ContractInformation.HttpProviderURL);
			_erc20Contract =
				ankrSDK.GetContract(
					ERC20ContractInformation.ContractAddress,
					ERC20ContractInformation.ABI);
			_eth = ankrSDK.Eth;

			_eventSubscriber = ankrSDK.GetSubscriber(ERC20ContractInformation.WsProviderURL);
			await _eventSubscriber.Connect();
		}
		
		private void Update()
		{
			_eventSubscriber.Update();
		}

		public async void CallMint()
		{
			var receipt = await _erc20Contract.CallMethod(MintMethodName, Array.Empty<object>());
			Debug.Log($"Receipt: {receipt}");

			var trx = await _eth.GetTransaction(receipt);

			Debug.Log($"Nonce: {trx.Nonce}");
		}
		
		public async void SendMint()
		{
			var filters = new EventFilterData
			{
				fromBlock = BlockParameter.CreateLatest(),
				toBlock = BlockParameter.CreateLatest()
			};
			Debug.Log("------ Message sending ------");
			var subscription = await _eventSubscriber.Subscribe(filters, ERC20ContractInformation.ContractAddress);
			Debug.Log(subscription);
			Debug.Log("------ Message sent ------");
		}
		
		public static void HandleSent(object sender, TransactionInput transaction)
		{
			Debug.Log("Transaction sent");
		}
		
		public static void HandleSending(object sender, TransactionInput transaction)
		{
			Debug.Log("Transaction is sending");
		}
		
		public void HandleTransactionHash(object sender, string transactionHash)
		{
			Debug.Log($"TsransactionHash: {transactionHash}");
		}

		public void HandleError(object sender, Exception exception)
		{
			Debug.LogError("Error: " + exception.Message);
		}

		public void HandleReceipt(object sender, TransactionReceipt receipt)
		{
			Debug.Log("Receipt: " + receipt.Status);
		}

		public async void GetBalance()
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = EthHandler.DefaultAccount
			};
			var balance = await _erc20Contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
			Debug.Log($"Balance: {balance}");
		}

		public async void GetEvents()
		{
			var filters = new EventFilterData
			{
				fromBlock = BlockParameter.CreateEarliest(),
				toBlock = BlockParameter.CreateLatest(),
			};
			var events = await _erc20Contract.GetAllChanges<TransferEventDTO>(filters);

			foreach (var ev in events)
			{
				Debug.Log(ev.Event.Value);
			}
		}
	}
}