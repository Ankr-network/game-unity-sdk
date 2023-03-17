using System.Numerics;
using AnkrSDK.Base;
using AnkrSDK.CommonUtils;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Data.ContractMessages.ERC1155;
using AnkrSDK.DTO;
using AnkrSDK.GameCharacterContract;
using AnkrSDK.Provider;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.WearableNFTExample
{
	public class WearableNFTExample : UseCaseBodyUI
	{
		private const int LogsBlockOffset = -10;
		private const string TransactionGasLimit = "1000000";
		private static readonly BigInteger BlueHatAddress = "0x00010000000000000000000000000000000000000000000000000000000001".HexToBigInteger(false);
		private static readonly BigInteger RedHatAddress = "0x00010000000000000000000000000000000000000000000000000000000002".HexToBigInteger(false);
		private static readonly BigInteger BlueShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000001".HexToBigInteger(false);
		private static readonly BigInteger WhiteShoesAddress = "0x00020000000000000000000000000000000000000000000000000000000003".HexToBigInteger(false);
		private static readonly BigInteger RedGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000002".HexToBigInteger(false);
		private static readonly BigInteger WhiteGlassesAddress = "0x00030000000000000000000000000000000000000000000000000000000003".HexToBigInteger(false);

		[SerializeField] private TMP_Text _text;

		[SerializeField] private Button _mintItemsButton;
		[SerializeField] private Button _mintCharacterButton;
		[SerializeField] private Button _getItemSetApprovalButton;
		[SerializeField] private Button _getCharacterBalanceButton;
		[SerializeField] private Button _getCharacterIDButton;
		[SerializeField] private Button _changeHatBlueButton;
		[SerializeField] private Button _changeHatRedButton;
		[SerializeField] private Button _getHatButton;

		private IContract _gameCharacterContract; //you can find the source in the mirage-smart-contract-example repo in contracts/GameCharacter.sol
		private IContract _gameItemContract; //you can find the source in the mirage-smart-contract-example repo in contracts/GameItem.sol

		private IEthHandler _ethHandler;

		private ABIStringLoader _abiLoader;
		private Web3 _web3;

		private void Awake()
		{
			_abiLoader = new ABIStringLoader("AnkrSDK/Examples/ABIs");
			_web3 = new Web3(WearableNFTContractInformation.ProviderHttpURL);
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

		public override void SetUseCaseBodyActive(bool isActive)
		{
			base.SetUseCaseBodyActive(isActive);

			if (isActive)
			{
				var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(WearableNFTContractInformation.ProviderHttpURL);
				var gameCharacterABI = _abiLoader.LoadAbi("GameCharacter");
				_gameCharacterContract = ankrSDK.GetContract(
					WearableNFTContractInformation.GameCharacterContractAddress, gameCharacterABI);
				var gameItemABI = _abiLoader.LoadAbi("GameItem");
				_gameItemContract = ankrSDK.GetContract(WearableNFTContractInformation.GameItemContractAddress,gameItemABI);
				
				
				
				_ethHandler = ankrSDK.Eth;
			}
		}

		private async UniTask MintItems()
		{
			const string mintBatchMethodName = "mintBatch";
			var itemsToMint = new[]
			{
				BlueHatAddress, RedHatAddress, BlueShoesAddress, WhiteShoesAddress, RedGlassesAddress, WhiteGlassesAddress
			};

			var itemsAmounts = new BigInteger[]
			{
				1, 2, 3, 4, 5, 6
			};
			var data = new byte[]
			{
			};

			var defaultAccount = await _ethHandler.GetDefaultAccount();
			
			var eventAwaiter = new EventAwaiter<BatchMintedEventDTO>(WearableNFTContractInformation.GameItemContractAddress, WearableNFTContractInformation.ProviderWssURL);
			var filterRequest = new EventFilterRequest<BatchMintedEventDTO>();
			filterRequest.AddTopic("To", defaultAccount);
			
			await eventAwaiter.StartWaiting(filterRequest);
			
			var receipt = await _gameItemContract.CallMethod(mintBatchMethodName,
				new object[]
				{
					defaultAccount, itemsToMint, itemsAmounts, data
				});

			UpdateUILogs($"Game Items Minted. Receipts : {receipt}");

			var eventDto = await eventAwaiter.Task;
			
			UpdateUILogs($"Event {eventDto.GetType()} received: {eventDto}");
		}

		private async UniTask<BigInteger> GetOffsetBlockNumber(int offset)
		{
			var blockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
			return blockNumber + new BigInteger(offset);
		}

		private async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";

			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var transactionHash =
				await _gameCharacterContract.CallMethod(safeMintMethodName, new object[]
				{
					defaultAccount
				});

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

		private async UniTask<BigInteger> GetBalanceERC1155(IContract contract, BigInteger id)
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var balanceOfMessage = new BalanceOfMessage
			{
				Account = defaultAccount, Id = id
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

		private async UniTask<bool> GetHasHatToken(BigInteger tokenAddress)
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

		private async UniTask ChangeHat(BigInteger hatAddress)
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
					characterId, BlueHatAddress
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