using System;
using System.Numerics;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.Scripts.ContractMessages;
using MirageSDK.Examples.Scripts.DTO;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace MirageSDK.Examples.Scripts.ERC20Example
{
	public class ERC20Example : MonoBehaviour
	{
		private const string MintMethodName = "mint";
		private IContract _erc20Contract;

		private void Start()
		{
			var mirageSDKWrapper = MirageSDKWrapper.GetInitializedInstance(ERC20ContractInformation.ProviderURL);
			_erc20Contract =
				mirageSDKWrapper.GetContract(ERC20ContractInformation.ContractAddress, ERC20ContractInformation.ABI);
		}

		public async void CallMint()
		{
			var receipt = await _erc20Contract.CallMethod(MintMethodName, Array.Empty<object>());
			Debug.Log($"Receipt: {receipt}");

			var trx = await _erc20Contract.GetTransactionInfo(receipt);

			Debug.Log($"Nonce: {trx.Nonce}");
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