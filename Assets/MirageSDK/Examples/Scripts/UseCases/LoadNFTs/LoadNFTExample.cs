using System.Numerics;
using MirageSDK.Base;
using MirageSDK.CommonUtils;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;
using MirageSDK.Data.ContractMessages.ERC721;
using MirageSDK.GameCharacterContract;
using MirageSDK.Provider;
using MirageSDK.WearableNFTExample;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.UseCases.LoadNFTs
{
	public class LoadNFTExample : UseCaseBodyUI
	{
		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private Button _loadNFTDataButton;

		[SerializeField]
		private Button _loadNFTMetaDataButton;

		[SerializeField]
		private ProviderInformationSO _providerInfo;

		[SerializeField]
		private ContractInformationSO _gameCharacterContractInfo;

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
			var sdkInstance = MirageSDKFactory.GetMirageSDKInstance(_providerInfo.HttpProviderURL);
			_gameCharacterContract = sdkInstance.GetContract(_gameCharacterContractInfo);
			StartAsync(sdkInstance).Forget();
		}

		private async UniTaskVoid StartAsync(IMirageSDK sdkInstance)
		{
			_activeSessionAccount = await sdkInstance.Eth.GetDefaultAccount();
		}

		public override void SetUseCaseBodyActive(bool isActive)
		{
			base.SetUseCaseBodyActive(isActive);
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

		private async UniTask<string> GetTokenMetaData(BigInteger tokenID)
		{
			var tokenUriMessage = new TokenURIMessage
			{
				TokenId = tokenID
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
				CharacterId = tokenID
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);

			return hatId;
		}

		private async UniTask<BigInteger> GetShoes(BigInteger tokenID)
		{
			var getShoesMessage = new GetShoesMessage
			{
				CharacterId = tokenID
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