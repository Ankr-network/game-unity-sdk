using System;
using System.Numerics;
using MirageSDK.Base;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Data.ContractMessages.ERC721;
using MirageSDK.DTO;
using MirageSDK.Provider;
using MirageSDK.UseCases;
using Cysharp.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace MirageSDK.ERC721Example
{
	public class ERC721Example : UseCaseBodyUI
	{
		private const string MintMethodName = "mint";
		private IContract _erc721Contract;
		private IEthHandler _eth;

		private void Start()
		{
			var sdkInstance = MirageSDKFactory.GetMirageSDKInstance(ERC721ContractInformation.ProviderURL);

			_erc721Contract =
				sdkInstance.GetContract(
					ERC721ContractInformation.ContractAddress,
					ERC721ContractInformation.ABI);
			_eth = sdkInstance.Eth;
		}

		public async void CallMint()
		{
			var receipt = await _erc721Contract.CallMethod(MintMethodName, Array.Empty<object>());
			Debug.Log($"Receipt: {receipt}");

			var trx = await _eth.GetTransaction(receipt);

			Debug.Log($"Nonce: {trx.Nonce}");
		}

		public async UniTaskVoid GetBalance()
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = await _eth.GetDefaultAccount()
			};
			var balance = await _erc721Contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
			Debug.Log($"Balance: {balance}");
		}

		public async UniTaskVoid GetEvents()
		{
			var filters = new EventFilterData()
			{
				FromBlock = BlockParameter.CreateEarliest(),
				ToBlock = BlockParameter.CreateLatest(),
				FilterTopic2 = new object[] { await _eth.GetDefaultAccount() }
			};
			var events = await _erc721Contract.GetEvents<TransferEventDTO>(filters);

			foreach (var ev in events)
			{
				Debug.Log(ev.Event.Value);
			}
		}
	}
}