using System;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.NEthereum;
using Nethereum.Web3;

namespace AnkrSDK.Mobile
{
	public class MobileWeb3Provider
	{
		private readonly WalletConnect2.WalletConnect2 _walletConnect;

		public MobileWeb3Provider()
		{
			_walletConnect = ConnectProvider<WalletConnect2.WalletConnect2>.GetConnect();
		}

		public IWeb3 CreateWeb3(string providerURI)
		{
			var client =
				_walletConnect.CreateProvider(
					new Uri(providerURI));
			var web3 = new Web3(client);
			return web3;
		}
	}
}