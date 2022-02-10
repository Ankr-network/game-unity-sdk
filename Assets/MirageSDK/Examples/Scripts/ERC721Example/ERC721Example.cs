using System;
using System.Numerics;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.Scripts.ContractMessages;
using MirageSDK.Examples.Scripts.DTO;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace MirageSDK.Examples.Scripts.ERC721Example
{
	public class ERC721Example : MonoBehaviour
	{
		private const string MintMethodName = "mint";
		private IContract _erc721Contract;

		private void Start()
		{
			var mirageSDKWrapper = MirageSDKWrapper.GetInitializedInstance(ERC721ContractInformation.ProviderURL);
			_erc721Contract =
				mirageSDKWrapper.GetContract(ERC721ContractInformation.ContractAddress, ERC721ContractInformation.ABI);
		}

		public async void CallMint()
		{
			var receipt = await _erc721Contract.CallMethod(MintMethodName, Array.Empty<object>());
			Debug.Log($"Receipt: {receipt}");

			var trx = await _erc721Contract.GetTransactionInfo(receipt);

			Debug.Log($"Nonce: {trx.Nonce}");
		}

		public async void GetBalance()
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = WalletConnect.ActiveSession.Accounts[0]
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