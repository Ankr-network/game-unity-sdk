using System;
using AnkrSDK.WalletConnectSharp.NEthereum;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
{
	public class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IWeb3 _web3Provider;
		public IEthHandler Eth { get; }

		internal AnkrSDKWrapper(string providerURI)
		{
			_web3Provider = CreateWeb3Provider(providerURI);
#if UNITY_WEBGL
			Eth = new EthHandlerWebGL();
#elif UNITY_IOS && UNITY_ANDROID		
			Eth = new EthHandlerMobile(_web3Provider);
#endif
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
#if UNITY_WEBGL
			return new Web3(providerURI);
#else
			var wcProtocol = WalletConnect.ActiveSession;
			var client = wcProtocol.CreateProvider(new Uri(providerURI));
			var web3Provider = new Web3(client);
			return web3Provider;
#endif
		}
		
		public static async UniTask Disconnect(bool waitForNewSession = true)
		{
			await WalletConnect.CloseSession(waitForNewSession);
		}
	}
}