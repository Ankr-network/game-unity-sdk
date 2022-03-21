using System;
using System.Numerics;
using AnkrSDK.Core.Data.ContractMessages.ERC721;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.DTO;
using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;

namespace AnkrSDK.Examples.ERC721Example
{
	public class ERC721Example : MonoBehaviour
	{
		private const string MintMethodName = "mint";
		private IContract _erc721Contract;
		private EthHandler _eth;

		private void Start()
		{
			var ankrSDKWrapper = AnkrSDKWrapper.GetSDKInstance(ERC721ContractInformation.ProviderURL);
			_erc721Contract =
				ankrSDKWrapper.GetContract(
					ERC721ContractInformation.ContractAddress,
					ERC721ContractInformation.ABI);
			_eth = ankrSDKWrapper.Eth;
		}

		public async void CallMint()
		{
			var receipt = await _erc721Contract.CallMethod(MintMethodName, Array.Empty<object>());
			Debug.Log($"Receipt: {receipt}");

			var trx = await _eth.GetTransaction(receipt);

			Debug.Log($"Nonce: {trx.Nonce}");
		}

		public async void GetBalance()
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = EthHandler.DefaultAccount
			};
			var balance = await _erc721Contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
			Debug.Log($"Balance: {balance}");
		}

		public async void GetEvents()
		{
			var events = await _erc721Contract.GetAllChanges<TransferEventDTO>();

			foreach (var ev in events)
			{
				Debug.Log(ev.Event.Value);
			}
		}
	}
}