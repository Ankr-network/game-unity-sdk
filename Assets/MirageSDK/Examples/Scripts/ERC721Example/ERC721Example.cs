using System;
using System.Numerics;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.ContractMessages.ERC721;
using MirageSDK.Examples.DTO;
using MirageSDK.WalletConnectSharp.Unity;
using UnityEngine;

namespace MirageSDK.Examples.ERC721Example
{
	public class ERC721Example : MonoBehaviour
	{
		private const string MintMethodName = "mint";
		private IContract _erc721Contract;
		private EthHandler _eth;

		private void Start()
		{
			var mirageSDKWrapper = MirageSDKWrapper.GetSDKInstance(ERC721ContractInformation.ProviderURL);
			_erc721Contract =
				mirageSDKWrapper.GetContract(
					ERC721ContractInformation.ContractAddress,
					ERC721ContractInformation.ABI);
			_eth = mirageSDKWrapper.Eth;
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