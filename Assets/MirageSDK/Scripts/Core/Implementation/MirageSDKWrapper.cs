using System;
using MirageSDK.Core.Infrastructure;
using MirageSDK.WalletConnectSharp.NEthereum;
using MirageSDK.WalletConnectSharp.Unity;
using Nethereum.Web3;

namespace MirageSDK.Core.Implementation
{
	public class MirageSDKWrapper : IMirageSDK
	{
		private readonly IWeb3 _web3Provider;

		public EthHandler Eth { get; }

		private MirageSDKWrapper(string providerURI)
		{
			_web3Provider = CreateWeb3Provider(providerURI);
			Eth = new EthHandler(_web3Provider);
		}

		/// <summary>
		///     Use this if you want to work with contracts from a single web3 provider.
		/// </summary>
		/// <param name="providerURI"></param>
		/// <returns></returns>
		public static IMirageSDK GetSDKInstance(string providerURI)
		{
			return new MirageSDKWrapper(providerURI);
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
