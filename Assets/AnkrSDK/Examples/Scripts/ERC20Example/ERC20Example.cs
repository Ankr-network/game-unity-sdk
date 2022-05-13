using System;
using System.Numerics;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Data.ContractMessages.ERC721;
using AnkrSDK.DTO;
using AnkrSDK.EventListenerExample;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
using AnkrSDK.UseCases;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AnkrSDK.ERC20Example
{
	public class ERC20Example : UseCase
	{
		private const string MintMethodName = "mint";
		private IContract _erc20Contract;
		private IEthHandler _eth;

		private void Start()
		{
			var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
			_erc20Contract =
				ankrSDK.GetContract(
					ERC20ContractInformation.ContractAddress,
					ERC20ContractInformation.ABI);
			_eth = ankrSDK.Eth;
		}

		public async void CallMint()
		{
			var receipt = await _erc20Contract.CallMethod(MintMethodName, Array.Empty<object>());
			Debug.Log($"Receipt: {receipt}");

			var trx = await _eth.GetTransaction(receipt);

			Debug.Log($"Nonce: {trx.Nonce}");
		}
		
		public void SendMint()
		{
			var evController = new TransactionEventDelegator();
			evController.OnTransactionSendBegin += HandleSending;
			evController.OnTransactionSendEnd += HandleSent;
			evController.OnTransactionHashReceived += HandleTransactionHash;
			evController.OnReceiptReceived += HandleReceipt;
			evController.OnError += HandleError;
			
			_erc20Contract.Web3SendMethod(MintMethodName, Array.Empty<object>(), evController);
		}

		private static void HandleSent(object sender, TransactionInput transaction)
		{
			Debug.Log("Transaction sent");
		}

		private static void HandleSending(object sender, TransactionInput transaction)
		{
			Debug.Log("Transaction is sending");
		}

		private static void HandleTransactionHash(object sender, string transactionHash)
		{
			Debug.Log($"TransactionHash: {transactionHash}");
		}

		private static void HandleError(object sender, Exception exception)
		{
			Debug.LogError("Error: " + exception.Message);
		}

		private static void HandleReceipt(object sender, TransactionReceipt receipt)
		{
			Debug.Log("Receipt: " + receipt.Status);
		}

		public async UniTaskVoid GetBalance()
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = await _eth.GetDefaultAccount()
			};
			var balance = await _erc20Contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
			Debug.Log($"Balance: {balance}");
		}

		public async UniTaskVoid GetEvents()
		{
			var filtersRequest = new EventFilterRequest<TransferEventDTO>
			{
				FromBlock = BlockParameter.CreateEarliest(),
				ToBlock = BlockParameter.CreateLatest()
			};
			filtersRequest.AddTopic("To", await _eth.GetDefaultAccount());
			
			var events = await _erc20Contract.GetEvents(filtersRequest);

			foreach (var ev in events)
			{
				Debug.Log(ev.Event.Value);
			}
		}
	}
}