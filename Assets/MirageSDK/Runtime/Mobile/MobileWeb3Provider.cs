using System;
using MirageSDK.Utils;
using MirageSDK.WalletConnectSharp.NEthereum;
using Nethereum.Web3;

namespace MirageSDK.Mobile
{
	public class MobileWeb3Provider
	{
		private readonly WalletConnectSharp.Unity.WalletConnect _walletConnect;

		public MobileWeb3Provider()
		{
			_walletConnect = ConnectProvider<WalletConnectSharp.Unity.WalletConnect>.GetConnect();
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