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
		private readonly IEthHandler _eth;

		public IEthHandler Eth
		{
			get
			{
				if (_eth != null)
				{
					return _eth;
				}

				throw new InvalidOperationException(
					$"Trying to use {nameof(IEthHandler)} before initialization completed. Use GetSDKInstance First");
			}
		}

		private AnkrSDKWrapper(string providerURI)
		{
			_web3Provider = CreateWeb3Provider(providerURI);
#if UNITY_WEBGL
			_eth = new EthHandlerWebGL();
#elif UNITY_IOS && UNITY_ANDROID		
			_eth = new EthHandlerMobile(_web3Provider);
#endif
		}

		/// <summary>
		///     Use this if you want to work with contracts from a single web3 provider.
		/// </summary>
		/// <param name="providerURI"></param>
		/// <returns></returns>
		public static IAnkrSDK GetSDKInstance(string providerURI)
		{
			return new AnkrSDKWrapper(providerURI);
		}

		/// <summary>
		/// Creates a contract using provided web3 instance.
		/// </summary>
		/// <param name="contractAddress">Contract address</param>
		/// <param name="contractABI">Contract ABI</param>
		/// <returns>Initialized contract handler</returns>
		public IContract GetContract(string contractAddress, string contractABI)
		{
			return new Contract(_web3Provider, Eth, contractAddress, contractABI);
		}

		public ContractEventSubscriber CreateSubscriber(string wsUrl)
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