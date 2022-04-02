using System.Numerics;
using AnkrSDK.Core.Data.ContractMessages.ERC721;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Utils;
using AnkrSDK.Examples.GameCharacterContract;
using AnkrSDK.Examples.WearableNFTExample;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.LoadNFTs
{
	public class LoadNFTExample : UseCase
	{
		[SerializeField] private TMP_Text _text;
		[SerializeField] private Button _loadNFTDataButton;
		[SerializeField] private Button _loadNFTMetaDataButton;
		private string _activeSessionAccount;

		private IContract _gameCharacterContract;

		private void Awake()
		{
			_loadNFTDataButton.onClick.AddListener(CallGetTokenData);
			_loadNFTMetaDataButton.onClick.AddListener(CallGetTokenMetaData);
		}

		private void OnDestroy()
		{
			_loadNFTDataButton.onClick.RemoveListener(CallGetTokenData);
			_loadNFTMetaDataButton.onClick.RemoveListener(CallGetTokenMetaData);
		}

		private void StartUseCaseExample()
		{
			var ankrSDKWrapper = AnkrSDKWrapper.GetSDKInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = ankrSDKWrapper.GetContract(
				WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI);
			_activeSessionAccount = EthHandler.DefaultAccount;
		}

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();
			StartUseCaseExample();
		}

		private async void CallGetTokenData()
		{
			await GetTokenData();
		}

		private async void CallGetTokenMetaData()
		{
			var tokenId = await GetFirstTokenId();
			await GetTokenMetaData(tokenId);
		}

		private async UniTask GetTokenData()
		{
			var tokenId = await GetFirstTokenId();

			if (tokenId != 0)
			{
				UpdateUILogs("NFTCharacter id:" + tokenId);

				var hatID = await GetHat(tokenId);
				if (hatID > 0)
				{
					UpdateUILogs("Has Hat id:" + hatID);
				}
				else
				{
					UpdateUILogs("Doesnt Have Hat");
				}

				var shoesID = await GetShoes(tokenId);
				if (shoesID > 0)
				{
					UpdateUILogs("Has Shoes id:" + shoesID);
				}
				else
				{
					UpdateUILogs("Doesnt Have Shoes");
				}
			}
		}

		private async UniTask<string> GetTokenMetaData(BigInteger tokenID)
		{
			var tokenUriMessage = new TokenURIMessage
			{
				TokenId = tokenID.ToString()
			};
			var tokenMetaData = await _gameCharacterContract.GetData<TokenURIMessage, string>(tokenUriMessage);

			UpdateUILogs("Token id:" + tokenID + " MetaData: {tokenMetaData}");

			return tokenMetaData;
		}

		private async UniTask<BigInteger> GetFirstTokenId()
		{
			var tokenBalance = await GetBalanceOf();

			if (tokenBalance > 0)
			{
				var tokenId = await _gameCharacterContract.TokenOfOwnerByIndex(_activeSessionAccount, 0);

				return tokenId;
			}

			return 0;
		}

		private async UniTask<BigInteger> GetHat(BigInteger tokenID)
		{
			var getHatMessage = new GetHatMessage
			{
				CharacterId = tokenID.ToString()
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);

			return hatId;
		}

		private async UniTask<BigInteger> GetShoes(BigInteger tokenID)
		{
			var getShoesMessage = new GetShoesMessage
			{
				CharacterId = tokenID.ToString()
			};
			var shoesID = await _gameCharacterContract.GetData<GetShoesMessage, BigInteger>(getShoesMessage);

			return shoesID;
		}

		private async UniTask<BigInteger> GetBalanceOf()
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = _activeSessionAccount
			};
			var balance = await _gameCharacterContract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);

			return balance;
		}

		private void UpdateUILogs(string log)
		{
			_text.text += "\n" + log;
			Debug.Log(log);
		}
	}
}