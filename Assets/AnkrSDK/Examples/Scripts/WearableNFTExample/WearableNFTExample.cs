using System.Numerics;
using AnkrSDK.Base;
using AnkrSDK.CommonUtils;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data.ContractMessages.ERC1155;
using AnkrSDK.GameCharacterContract;
using AnkrSDK.Provider;
using AnkrSDK.UseCases;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.WearableNFTExample
{
	public class WearableNFTExample : UseCaseBodyUI
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

		private IContract _gameCharacterContract; //https://github.com/mirage-xyz/mirage-smart-contract-example/blob/cdf3d72668ea8de19b9ad410f96d7409c3b2f09e/composable-nft/contracts/GameCharacter.sol
		private IContract _gameItemContract; //https://github.com/mirage-xyz/mirage-smart-contract-example/blob/cdf3d72668ea8de19b9ad410f96d7409c3b2f09e/composable-nft/contracts/GameItem.sol

		private IEthHandler _ethHandler;

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

		public override void SetUseCaseBodyActive(bool active)
		{
			base.SetUseCaseBodyActive(active);

			if (active)
			{
				var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(WearableNFTContractInformation.ProviderURL);
				_gameCharacterContract = ankrSDK.GetContract(
					WearableNFTContractInformation.GameCharacterContractAddress,
					WearableNFTContractInformation.GameCharacterABI);
				_gameItemContract = ankrSDK.GetContract(WearableNFTContractInformation.GameItemContractAddress,
					WearableNFTContractInformation.GameItemABI);
				_ethHandler = ankrSDK.Eth;
			}

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
				WhiteGlassesAddress
			};
			var itemsAmounts = new[]
			{
				1, 2, 3, 4, 5, 6
			};
			var data = new byte[] { };

			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var receipt = await _gameItemContract.CallMethod(mintBatchMethodName,
				new object[] { defaultAccount, itemsToMint, itemsAmounts, data });

			UpdateUILogs($"Game Items Minted. Receipts : {receipt}");
		}

		private async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";

			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var transactionHash =
				await _gameCharacterContract.CallMethod(safeMintMethodName, new object[] { defaultAccount });

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
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var balance = await _gameCharacterContract.BalanceOf(defaultAccount);

			UpdateUILogs($"Number of NFTs Owned: {balance}");
			return balance;
		}

		private async UniTask<BigInteger> GetBalanceERC1155(IContract contract, string id)
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var balanceOfMessage = new BalanceOfMessage
			{
				Account = defaultAccount,
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
				var defaultAccount = await _ethHandler.GetDefaultAccount();
				var tokenId =
					await _gameCharacterContract.TokenOfOwnerByIndex(defaultAccount, 0);

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
			
			if (characterID.Equals(-1))
			{
				UpdateUILogs("ERROR : CharacterID or HatID is null");
				return -1;
			}
			
			var getHatMessage = new GetHatMessage
			{
				CharacterId = characterID
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);
			var hexHatID = AnkrSDKHelper.StringToBigInteger(hatId.ToString());

			UpdateUILogs($"Hat Id: {hexHatID}");

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