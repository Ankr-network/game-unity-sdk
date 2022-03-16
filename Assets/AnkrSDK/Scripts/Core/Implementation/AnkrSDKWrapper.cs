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

		private AnkrSDKWrapper(string providerURI)
		{
			_web3Provider = CreateWeb3Provider(providerURI);
			Eth = new EthHandler(_web3Provider);
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

		private static IWeb3 CreateWeb3Provider(string providerURI)
		{
			if (WalletConnect.Instance == null || WalletConnect.Instance.Session == null)
			{
				throw new ArgumentNullException(nameof(WalletConnect.Instance),
					"WalletConnect should be initialized before creating web3Provider");
			}

			var wcProtocol = WalletConnect.Instance.Session;
			var client = wcProtocol.CreateProvider(new Uri(providerURI));
			var web3Provider = new Web3(client);
			return web3Provider;
		}
	}
}
