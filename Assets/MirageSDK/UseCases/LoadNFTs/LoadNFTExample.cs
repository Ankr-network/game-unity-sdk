using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.Scripts.ContractMessages;
using MirageSDK.Examples.Scripts.WearableNFTExample;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Unity;

namespace MirageSDK.Examples.Scripts.LoadNFTExample
{
    public class LoadNFTExample : MonoBehaviour
    {

        private const string TransactionGasLimit = "1000000";

        private IContract _gameCharacterContract;

        [SerializeField] private Text _text;

        private void Start()
        {
            var mirageSDKWrapper = MirageSDKWrapper.GetSDKInstance(WearableNFTContractInformation.ProviderURL);
            _gameCharacterContract = mirageSDKWrapper.GetContract(
                WearableNFTContractInformation.GameCharacterContractAddress,
                WearableNFTContractInformation.GameCharacterABI);
        }

        public async void CallGetTokenInfo()
        {
            await GetTokenInfo();
        }
        
        private async UniTask GetTokenInfo()
        {
            var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
            const string tokenOfOwnerByIndex = "tokenOfOwnerByIndex";
            var tokenBalance = await GetBalance();
            var token = await _gameCharacterContract.CallMethod(tokenOfOwnerByIndex, new object[] { activeSessionAccount, tokenBalance - 1 });
			
            Debug.Log($"Address owns this Token : {token}");
            UpdateUILogs($"Address owns this Token : {token}");
        }
        
        
        private async UniTask<BigInteger> GetBalance()
        {
            var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
            var balanceOfMessage = new BalanceOfMessage
            {
                Owner = activeSessionAccount
            };
            var balance = await _gameCharacterContract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
            Debug.Log($"Number of Character NFTs Owned: {balance}");
            UpdateUILogs($"Number of Character NFTs Owned: {balance}");
            return balance;
        }
        
        private void UpdateUILogs(string log)
        {
            _text.text = log;
        }
    }
}