using System;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
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
            var wcProtocol = WalletConnect.Instance.Protocol;
            _client =
                wcProtocol.CreateProvider(new Uri(_provider_url));
            _provider = new Nethereum.Web3.Web3(_client);
        }

        public Contract GetContract(string address, string abi)
        {
            return new Contract(_provider, _client, address, abi);
        }
    }
}