using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.Scripts.ContractMessages;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace MirageSDK.Examples.Scripts.WearableNFTExample
{
	/// <summary>
	/// You need to have a minter role for this example to work.
	/// </summary>
	public class WearableNFTExample : MonoBehaviour
	{
		private const string TransactionGasLimit = "1000000";
		private const string BlueHatAddress = "0x00010000000000000000000000000000000000000000000000000000000001";
		private const string RedHatAddress = "0x00010000000000000000000000000000000000000000000000000000000002";
		private const string BlueShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000001";
		private const string WhiteShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000003";
		private const string RedGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000002";
		private const string WhiteGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000003";
		
		private IContract _gameCharacterContract;
		private IContract _gameItemContract;

		private void Start()
		{
			var web3 = MirageSDKWrapper.GetInitializedInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = web3.GetContract(WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI);
			_gameItemContract = web3.GetContract(WearableNFTContractInformation.GameItemContractAddress,
				WearableNFTContractInformation.GameItemABI);
		}

		public async void RunExample()
		{
			await MintItems();
			await MintCharacter();
			await GameItemSetApproval();
			await GetTokenInfo();
			await ChangeHat(BlueHatAddress);
			await GetHat();
			await ChangeHat(RedHatAddress);
			await GetHat();
		}

		private async UniTask MintItems()
		{
			const string mintBatchMethodName = "mintBatch";
			var itemsToMint = new[]
			{
				BlueHatAddress,
				RedHatAddress,
				BlueShoesAddress,
				WhiteShoesAddress,
				RedGlassesAddress,
				WhiteGlassesAddress,
			};
			var itemsAmounts = new[]
			{
				1, 2, 3, 4, 5, 6
			};

			var receipt = await _gameItemContract.CallMethod(mintBatchMethodName,
				new object[] { itemsToMint, itemsAmounts });
			Debug.Log($"Game Items Minted. Receipts : {receipt}");
		}

		private async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";

			var transactionHash = await _gameCharacterContract.CallMethod(safeMintMethodName);
			Debug.Log($"Game Character Minted. Hash : {transactionHash}");
		}

		private async UniTask GameItemSetApproval()
		{
			const string setApprovalMethodName = "setApprovalForAll";
			
			var transactionHash = await _gameItemContract.CallMethod(setApprovalMethodName, new object[]
			{
				WearableNFTContractInformation.GameCharacterContractAddress,
				true
			});
			
			Debug.Log($"Game Character approved. Hash : {transactionHash}");
		}

		private async UniTask<BigInteger> GetBalance()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = activeSessionAccount
			};
			var balance = await _gameCharacterContract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
			Debug.Log($"Balance: {balance}");
			return balance;
		}

		private async UniTask GetTokenInfo()
		{
			const string tokenOfOwnerByIndex = "tokenOfOwnerByIndex";
			var tokenBalance = await GetBalance();
			var token = await _gameCharacterContract.CallMethod(tokenOfOwnerByIndex, new object[] { tokenBalance - 1 });
			
			Debug.Log($"Minted Token token : {token}");
		}

		private async UniTask ChangeHat(string hatAddress)
		{
			const string changeHatMethodName = "changeHat";
			
			var transactionHash = await _gameItemContract.CallMethod(changeHatMethodName, new object[]
			{
				hatAddress
			}, TransactionGasLimit);

			Debug.Log($"Hat Changed. Hash : {transactionHash}");
		}
		
		private async UniTask<BigInteger> GetHat()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var balanceOfMessage = new ItemMessage
			{
				CharacterId = activeSessionAccount
			};
			var hatId = await _gameCharacterContract.GetData<ItemMessage, BigInteger>(balanceOfMessage);
			Debug.Log($"Hat Id: {hatId}");
			return hatId;
		}
	}
}