using System;
using AnkrSDK.WalletConnectSharp.NEthereum;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
{
	public class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IWeb3 _web3Provider;

		public EthHandler Eth { get; }

		internal AnkrSDKWrapper(string providerURI)
		{
			_web3Provider = CreateWeb3Provider(providerURI);
			Eth = new EthHandler(_web3Provider);
		}

		public IContract GetContract(string contractAddress, string contractABI)
		{
			return new Contract(_web3Provider, Eth, contractAddress, contractABI);
		}

		public IContractEventSubscriber CreateSubscriber(string wsUrl)
		{
			return new ContractEventSubscriber(wsUrl);
		}

		private static IWeb3 CreateWeb3Provider(string providerURI)
		{
			var wcProtocol = WalletConnect.ActiveSession;
			var client = wcProtocol.CreateProvider(new Uri(providerURI));
			var web3Provider = new Web3(client);
			return web3Provider;
		}
	}
}