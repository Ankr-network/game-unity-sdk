using AnkrSDK.Plugins.WalletConnectSharp.NEthereum;
using AnkrSDK.Plugins.WalletConnectSharp.Unity;
using AnkrSDK.Utils;
using Nethereum.Web3;

namespace AnkrSDK.Mobile
{
	public class MobileWeb3Provider
	{
		private readonly WalletConnect _walletConnect;

		public MobileWeb3Provider()
		{
			_walletConnect = ConnectProvider<WalletConnect>.GetConnect();
		}

		public IWeb3 CreateWeb3(string providerURI)
		{
			var client =
				_walletConnect.CreateProvider(
					new System.Uri(providerURI));
			var web3 = new Web3(client);
			return web3;
		}
	}
}