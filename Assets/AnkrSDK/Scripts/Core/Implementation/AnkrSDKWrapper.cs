using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;
using Nethereum.Web3;

namespace AnkrSDK.Core.Implementation
{
	internal class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IWeb3 _web3Provider;
		public IEthHandler Eth { get; }

		internal AnkrSDKWrapper(string providerURI)
		{
			_web3Provider = CreateWeb3Provider(providerURI);
		#if UNITY_WEBGL && !UNITY_EDITOR
			Eth = new EthHandlerWebGL();
		#else
			Eth = new EthHandler(_web3Provider);
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
		#if UNITY_WEBGL && !UNITY_EDITOR
			return new Web3(providerURI);
		#else
			var wcProtocol = WalletConnect.ActiveSession;
			var client =
				WalletConnectSharp.NEthereum.WalletConnectNEthereumExtensions.CreateProvider(wcProtocol,
					new System.Uri(providerURI));
			var web3Provider = new Web3(client);
			return web3Provider;
		#endif
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return WalletConnect.CloseSession(waitForNewSession);
		}
	}
}