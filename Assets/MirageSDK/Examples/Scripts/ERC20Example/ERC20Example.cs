using System;
using System.Numerics;
using MirageSDK.Core.Data;
using MirageSDK.Core.Events;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.ContractMessages.ERC721;
using MirageSDK.Examples.DTO;
using MirageSDK.WalletConnectSharp.Unity;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace MirageSDK.Examples.ERC20Example
{
	public class ERC20Example : MonoBehaviour
	{
		private const string MintMethodName = "mint";
		private IContract _erc20Contract;
		private EthHandler _eth;

		private void Start()
		{
			var mirageSDKWrapper = MirageSDKWrapper.GetSDKInstance(ERC20ContractInformation.ProviderURL);
			_erc20Contract =
				mirageSDKWrapper.GetContract(
					ERC20ContractInformation.ContractAddress,
					ERC20ContractInformation.ABI);
			_eth = mirageSDKWrapper.Eth;
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
			var evController = new EventController();
			evController.OnSending += HandleSending;
			evController.OnSent += HandleSent;
			evController.OnTransactionHash += HandleTransactionHash;
			evController.OnReceipt += HandleReceipt;
			evController.OnError += HandleError;
			
			_erc20Contract.Web3SendMethod("mint", Array.Empty<object>(), evController);
		}
		
		public void HandleSent(object sender, TransactionInput transaction)
		{
			Debug.Log($"Transaction sent");
		}
		
		public void HandleSending(object sender, TransactionInput transaction)
		{
			Debug.Log($"Transaction is sending");
		}

		public void HandleTransactionHash(object sender, string transactionHash)
		{
			Debug.Log($"transactionHash: {transactionHash}");
		}

		public void HandleReceipt(object sender, TransactionReceipt receipt)
		{
			Debug.Log("Receipt: " + receipt.Status);
		}
		
		public void HandleError(object sender, Exception err)
		{
			Debug.Log("Error: " + err.Message);
		}

		public async void GetBalance()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = activeSessionAccount
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
				filterTopic1 = new object[] { "Transfer" }
			};
			var events = await _erc20Contract.GetAllChanges<TransferEventDTO>(filters);


			foreach (var ev in events)
			{
				Debug.Log(ev.Event.Value);
			}
		}
	}
}