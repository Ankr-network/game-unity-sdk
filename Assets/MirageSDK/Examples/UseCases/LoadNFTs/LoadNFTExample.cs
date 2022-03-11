using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.ContractMessages.ERC721;
using MirageSDK.Examples.ContractMessages.GameCharacterContract;
using MirageSDK.Examples.WearableNFTExample;
using MirageSDK.WalletConnectSharp.Unity;
using TMPro;
using UnityEngine;

namespace MirageSDK.UseCases.LoadNFTs
{
	public class LoadNFTExample : MonoBehaviour
	{
		[SerializeField] private TMP_Text _text;

		private IContract _gameCharacterContract;

		private void Start()
		{
			var mirageSDKWrapper = MirageSDKWrapper.GetSDKInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = mirageSDKWrapper.GetContract(
				WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI);
		}

		public async void CallGetTokenData()
		{
			await GetTokenData();
		}

		public async void CallGetTokenMetaData()
		{
			var tokenId = await GetFirstTokenId();
			await GetTokenMetaData(tokenId);
		}

		private async UniTask GetTokenData()
		{
			var tokenId = await GetFirstTokenId();
			if (tokenId != 0)
			{
				var hatID = await GetHat(tokenId);
				var shoesID = await GetShoes(tokenId);
			}
		}

		private async UniTask<string> GetTokenMetaData(BigInteger tokenID)
		{
			var tokenUriMessage = new TokenURIMessage
			{
				TokenId = tokenID.ToString()
			};
			var tokenMetaData = await _gameCharacterContract.GetData<TokenURIMessage, string>(tokenUriMessage);

			UpdateUILogs($"Token MetaData: {tokenMetaData}");

			return tokenMetaData;
		}

		private async UniTask<BigInteger> GetFirstTokenId()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var tokenBalance = await GetBalanceOf();

			if (tokenBalance != 0)
			{
				var tokenOfOwnerByIndexMessage = new TokenOfOwnerByIndexMessage
				{
					Owner = activeSessionAccount, Index = tokenBalance - 1
				};
				var tokenId =
					await _gameCharacterContract.GetData<TokenOfOwnerByIndexMessage, BigInteger>(
						tokenOfOwnerByIndexMessage);

				UpdateUILogs($"First Token ID : {tokenId}");
				return tokenId;
			}
			else
			{
				UpdateUILogs("No NFT of this type in the wallet");
				return 0;
			}
		}

		private async UniTask<BigInteger> GetHat(BigInteger tokenID)
		{
			var getHatMessage = new GetHatMessage
			{
				CharacterId = tokenID.ToString()
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);

			UpdateUILogs($"Hat Id: {hatId}");

			return hatId;
		}

		private async UniTask<BigInteger> GetShoes(BigInteger tokenID)
		{
			var getShoesMessage = new GetShoesMessage
			{
				CharacterId = tokenID.ToString()
			};
			var shoesID = await _gameCharacterContract.GetData<GetShoesMessage, BigInteger>(getShoesMessage);

			UpdateUILogs($"Shoes Id: {shoesID}");

			return shoesID;
		}

		private async UniTask<BigInteger> GetBalanceOf()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = activeSessionAccount
			};
			var balance = await _gameCharacterContract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);

			UpdateUILogs($"Number of Character NFTs Owned: {balance}");
			return balance;
		}

		private void UpdateUILogs(string log)
		{
			_text.text += "\n" + log;
			Debug.Log(log);
		}
	}
}