using System.Numerics;
using MirageSDK.Base;
using MirageSDK.CommonUtils;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Data.ContractMessages.ERC1155;
using MirageSDK.DTO;
using MirageSDK.GameCharacterContract;
using MirageSDK.Provider;
using MirageSDK.Utils;
using Cysharp.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.WearableNFTExample
{
	public class WearableNFTExample : UseCaseBodyUI
	{
		private const int LogsBlockOffset = -10;
		private const string TransactionGasLimit = "1000000";

		private static readonly BigInteger BlueHatId =
			"0x00010000000000000000000000000000000000000000000000000000000001".HexToBigInteger(false);

		private static readonly BigInteger RedHatId =
			"0x00010000000000000000000000000000000000000000000000000000000002".HexToBigInteger(false);

		private static readonly BigInteger BlueShoesId =
			"0x00020000000000000000000000000000000000000000000000000000000001".HexToBigInteger(false);

		private static readonly BigInteger WhiteShoesId =
			"0x00020000000000000000000000000000000000000000000000000000000003".HexToBigInteger(false);

		private static readonly BigInteger RedGlassesId =
			"0x00030000000000000000000000000000000000000000000000000000000002".HexToBigInteger(false);

		private static readonly BigInteger WhiteGlassesId =
			"0x00030000000000000000000000000000000000000000000000000000000003".HexToBigInteger(false);

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private Button _mintItemsButton;

		[SerializeField]
		private Button _mintCharacterButton;

		[SerializeField]
		private Button _getItemSetApprovalButton;

		[SerializeField]
		private Button _getCharacterBalanceButton;

		[SerializeField]
		private Button _getCharacterIDButton;

		[SerializeField]
		private Button _changeHatBlueButton;

		[SerializeField]
		private Button _changeHatRedButton;

		[SerializeField]
		private Button _getHatButton;

		[SerializeField]
		private ContractInformationSO _gameItemContractInfo;

		[SerializeField]
		private ContractInformationSO _gameCharacterContractInfo;

		[SerializeField]
		private ProviderInformationSO _providerInfo;

		private IEthHandler _ethHandler;

		private IContract
			_gameCharacterContract; //you can find the source in the mirage-smart-contract-example repo in contracts/GameCharacter.sol

		private IContract
			_gameItemContract; //you can find the source in the mirage-smart-contract-example repo in contracts/GameItem.sol

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

		public override void SetUseCaseBodyActive(bool isActive)
		{
			base.SetUseCaseBodyActive(isActive);

			if (isActive)
			{
				var sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_providerInfo.HttpProviderURL);

				_gameCharacterContract = sdkInstance.GetContract(_gameCharacterContractInfo);

				_gameItemContract = sdkInstance.GetContract(_gameItemContractInfo);
				_ethHandler = sdkInstance.Eth;
			}
		}

		private async UniTask MintItems()
		{
			const string mintBatchMethodName = "mintBatch";
			var itemsToMint = new[]
			{
				BlueHatId, RedHatId, BlueShoesId, WhiteShoesId, RedGlassesId, WhiteGlassesId
			};

			var itemsAmounts = new BigInteger[]
			{
				1, 2, 3, 4, 5, 6
			};
			var data = new byte[]
			{
			};

			var defaultAccount = await _ethHandler.GetDefaultAccount();
			var transactionHash = await _gameItemContract.CallMethod(mintBatchMethodName,
				new object[]
				{
					defaultAccount, itemsToMint, itemsAmounts, data
				});

			UpdateUILogs($"Game Items Minted. Transaction hash : {transactionHash}");

			//example of decoding events from the transaction
			var transactionReceipt = await _ethHandler.GetTransactionReceipt(transactionHash);
			var eventLogs = transactionReceipt.DecodeAllEvents<BatchMintedEventDTO>();
			foreach (var eventLog in eventLogs)
			{
				var eventDto = eventLog.Event;
				UpdateUILogs($"Event {eventDto.GetType()} received: {eventDto}");
			}
		}

		private async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";

			var defaultAccount = await _ethHandler.GetDefaultAccount();

			var eventAwaiter = new EventAwaiter<SafeMintedEventDTO>(
				_gameCharacterContractInfo.ContractAddress,
				_providerInfo.WsProviderURL);
			var filterRequest = new EventFilterRequest<SafeMintedEventDTO>();
			filterRequest.AddTopic("To", defaultAccount);

			await eventAwaiter.StartWaiting(filterRequest);

			var transactionHash =
				await _gameCharacterContract.CallMethod(safeMintMethodName, new object[]
				{
					defaultAccount
				});

			UpdateUILogs($"Game Character Minted. Hash : {transactionHash}");

			//example of awaiting particular filtered event
			var eventDto = await eventAwaiter.ReceiveEventTask;
			UpdateUILogs($"Event {eventDto.GetType()} received: {eventDto}");
		}

		//Cant be called by the operator
		private async UniTask GameItemSetApproval()
		{
			var defaultAccount = await _ethHandler.GetDefaultAccount();

			var transactionHash = await _gameItemContract.SetApprovalForAll(
				defaultAccount,
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

		private async UniTask ChangeHat(BigInteger newHatId)
		{
			const string changeHatMethodName = "changeHat";
			var characterId = await GetCharacterTokenId();

			var hasHat = await GetHasHatToken(newHatId);

			if (!hasHat || characterId.Equals(-1))
			{
				UpdateUILogs("ERROR : CharacterID or HatID is null");
			}
			else
			{
				var eventAwaiter = new EventAwaiter<HatChangedEventDTO>(
					_gameCharacterContractInfo.ContractAddress,
					_providerInfo.WsProviderURL);
				var filterRequest = new EventFilterRequest<HatChangedEventDTO>();
				filterRequest.AddTopic("CharacterId", characterId);

				await eventAwaiter.StartWaiting(filterRequest);

				var transactionHash = await _gameCharacterContract.CallMethod(changeHatMethodName, new object[]
				{
					characterId, newHatId
				}, TransactionGasLimit);

				UpdateUILogs($"Hat Changed. Hash : {transactionHash}");

				var eventDto = await eventAwaiter.ReceiveEventTask;
				UpdateUILogs($"Event {eventDto.GetType()} received: {eventDto}");
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
			var hexHatID = MirageSDKHelper.StringToBigInteger(hatId.ToString());

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
			await ChangeHat(RedHatId);
		}

		private async void ChangeBlueHatCall()
		{
			await ChangeHat(BlueHatId);
		}

		private async void GetHatCall()
		{
			await GetHat();
		}

		#endregion
	}
}