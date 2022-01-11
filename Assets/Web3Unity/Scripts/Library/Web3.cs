using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.Signer;
using Nethereum.Web3;
using UnityEngine;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;

namespace Web3Unity.Scripts.Library
{
    public class Web3
    {
        private IWeb3 _provider;
        private IClient _client;
        private string _provider_url;

        public Web3(string provider_url)
        {
            _provider_url = provider_url;
            Initialize();
        }
        
        public void Initialize()
        {
            // "https://rinkeby.infura.io/v3/c75f2ce78a4a4b64aa1e9c20316fda3e"
            Debug.Log(WalletConnect.Instance);
            Debug.Log(WalletConnect.Instance.Protocol);
            var wcProtocol = WalletConnect.Instance.Protocol;
            _client =
                wcProtocol.CreateProvider(new Uri(_provider_url));
            _provider = new Nethereum.Web3.Web3(_client);
        }

        public Contract GetContract(string address, string abi)
        {
            return new Contract(_provider, _client, address, abi);
        }

        public Task<string> Sign(string message)
        {
            Debug.Log("-------------------------- Hey men --------------------------");
            Debug.Log($"Address: {WalletConnect.ActiveSession.Accounts[0]}");
            return WalletConnect.ActiveSession.EthSign(WalletConnect.ActiveSession.Accounts[0], message);
        }

        public string CheckSignature(string message, string signature)
        {
            var signer1 = new EthereumMessageSigner();
            return signer1.EncodeUTF8AndEcRecover(message, signature);
        }
    }
}