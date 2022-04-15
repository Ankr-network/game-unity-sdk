using System.Numerics;
using AnkrSDK.Core.Data.ContractMessages.ERC1155;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Examples.GameCharacterContract;
using AnkrSDK.Examples.WearableNFTExample;
using AnkrSDK.UseCases;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.WearableNFTExample
{
	public class WearableNFTExample : UseCase
	{
		private const string TransactionGasLimit = "1000000";
		private const string BlueHatAddress = "0x00010000000000000000000000000000000000000000000000000000000001";
		private const string RedHatAddress = "0x00010000000000000000000000000000000000000000000000000000000002";
		private const string BlueShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000001";
		private const string WhiteShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000003";
		private const string RedGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000002";
		private const string WhiteGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000003";

		[SerializeField] private TMP_Text _text;

		[SerializeField] private Button _mintItemsButton;
		[SerializeField] private Button _mintCharacterButton;
		[SerializeField] private Button _getItemSetApprovalButton;
		[SerializeField] private Button _getCharacterBalanceButton;
		[SerializeField] private Button _getCharacterIDButton;
		[SerializeField] private Button _changeHatBlueButton;
		[SerializeField] private Button _changeHatRedButton;
		[SerializeField] private Button _getHatButton;
		private string _activeSessionAccount;

		private IContract _gameCharacterContract;
		private IContract _gameItemContract;

		private void Awake()
		{
			SubscribeButtonLinks();
		}

		private void OnDestroy()
		{
			UnsubscribeButtonLinks();
		}

		private void SubscribeButtonLinks()
		{
			_mintItemsButton.onClick.AddListener(MintItemsCall);
			_mintCharacterButton.onClick.AddListener(MintCharacterCall);
			_getItemSetApprovalButton.onClick.AddListener(GameItemSetApprovalCall);
			_getCharacterBalanceButton.onClick.AddListener(GetCharacterBalanceCall);
			_getCharacterIDButton.onClick.AddListener(GetCharacterIdCall);
			_changeHatBlueButton.onClick.AddListener(ChangeBlueHatCall);
			_changeHatRedButton.onClick.AddListener(ChangeRedHatCall);
			_getHatButton.onClick.AddListener(GetHatCall);
		}

		private void UnsubscribeButtonLinks()
		{
			_mintItemsButton.onClick.RemoveListener(MintItemsCall);
			_mintCharacterButton.onClick.RemoveListener(MintCharacterCall);
			_getItemSetApprovalButton.onClick.RemoveListener(GameItemSetApprovalCall);
			_getCharacterBalanceButton.onClick.RemoveListener(GetCharacterBalanceCall);
			_getCharacterIDButton.onClick.RemoveListener(GetCharacterIdCall);
			_changeHatBlueButton.onClick.RemoveListener(ChangeBlueHatCall);
			_changeHatRedButton.onClick.RemoveListener(ChangeRedHatCall);
			_getHatButton.onClick.RemoveListener(GetHatCall);
		}

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();

			var ankrSDK = AnkrSDKWrapper.GetSDKInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = ankrSDK.GetContract(
				WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI);
			_gameItemContract = ankrSDK.GetContract(WearableNFTContractInformation.GameItemContractAddress,
				WearableNFTContractInformation.GameItemABI);
			_activeSessionAccount = EthHandler.DefaultAccount;
		}

		private async UniTask MintItems()
		{
			const string mintBatchMethodName = "mintBatch";
			const string mintBatchToAddress = "0x510c522ebCC6Eb376839E0CFf5D57bb2F422EB8b";
			var itemsToMint = new[]
			{
				BlueHatAddress,
				RedHatAddress,
				BlueShoesAddress,
				WhiteShoesAddress,
				RedGlassesAddress,
				WhiteGlassesAddress
			};
			var itemsAmounts = new[]
			{
				1, 2, 3, 4, 5, 6
			};
			var data = new byte[] { };

			var receipt = await _gameItemContract.CallMethod(mintBatchMethodName,
				new object[] {mintBatchToAddress, itemsToMint, itemsAmounts, data});

			UpdateUILogs($"Game Items Minted. Receipts : {receipt}");
		}

		private async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";

			var transactionHash =
				await _gameCharacterContract.CallMethod(safeMintMethodName, new object[] {_activeSessionAccount});

			UpdateUILogs($"Game Character Minted. Hash : {transactionHash}");
		}

		//Cant be called by the operator
		private async UniTask GameItemSetApproval()
		{
			var transactionHash = await _gameItemContract.SetApprovalForAll(
				WearableNFTContractInformation.GameCharacterContractAddress,
				true);

			UpdateUILogs($"Game Items approved. Hash : {transactionHash}");
		}

		private async UniTask<BigInteger> GetCharacterBalance()
		{
			var balance = await _gameCharacterContract.BalanceOf(_activeSessionAccount);

			UpdateUILogs($"Number of NFTs Owned: {balance}");
			return balance;
		}

		private async UniTask<BigInteger> GetBalanceERC1155(IContract contract, string id)
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Account = _activeSessionAccount,
				Id = id
			};
			var balance =
				await contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);

			UpdateUILogs($"Number of NFTs Owned: {balance}");
			return balance;
		}

		private async UniTask<BigInteger> GetCharacterTokenId()
		{
			var tokenBalance = await GetCharacterBalance();

			if (tokenBalance > 0)
			{
				var tokenId =
					await _gameCharacterContract.TokenOfOwnerByIndex(_activeSessionAccount, 0);

				UpdateUILogs($"GameCharacter tokenId  : {tokenId}");

				return tokenId;
			}

			UpdateUILogs("You dont own any of these tokens.");
			return -1;
		}

		private async UniTask<bool> GetHasHatToken(string tokenAddress)
		{
			var tokenBalance = await GetBalanceERC1155(_gameItemContract, tokenAddress);

			if (tokenBalance > 0)
			{
				UpdateUILogs("You have " + tokenBalance + " hats");
				return true;
			}

			UpdateUILogs("You dont have any Hat Item");
			return false;
		}

		private async UniTask ChangeHat(string hatAddress)
		{
			const string changeHatMethodName = "changeHat";
			var characterId = await GetCharacterTokenId();

			var hasHat = await GetHasHatToken(hatAddress);

			if (!hasHat || characterId.Equals(-1))
			{
				UpdateUILogs("ERROR : CharacterID or HatID is null");
			}
			else
			{
				var transactionHash = await _gameCharacterContract.CallMethod(changeHatMethodName, new object[]
				{
					characterId,
					BlueHatAddress
				}, TransactionGasLimit);

				UpdateUILogs($"Hat Changed. Hash : {transactionHash}");
			}
		}

		private async UniTask<BigInteger> GetHat()
		{
			var characterID = await GetCharacterTokenId();
			var getHatMessage = new GetHatMessage
			{
				CharacterId = characterID.ToString()
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);

			UpdateUILogs($"Hat Id: {hatId}");

			return hatId;
		}

		private void UpdateUILogs(string log)
		{
			_text.text += "\n" + log;
			Debug.Log(log);
		}

		#region Button Calls

		private async void MintItemsCall()
		{
			await MintItems();
		}

		private async void MintCharacterCall()
		{
			await MintCharacter();
		}

		private async void GetCharacterBalanceCall()
		{
			await GetCharacterBalance();
		}

		private async void GameItemSetApprovalCall()
		{
			await GameItemSetApproval();
		}

		private async void GetCharacterIdCall()
		{
			await GetCharacterTokenId();
		}

		private async void ChangeRedHatCall()
		{
			await ChangeHat(RedHatAddress);
		}

		private async void ChangeBlueHatCall()
		{
			await ChangeHat(BlueHatAddress);
		}

		private async void GetHatCall()
		{
			await GetHat();
		}

		#endregion
	}
}