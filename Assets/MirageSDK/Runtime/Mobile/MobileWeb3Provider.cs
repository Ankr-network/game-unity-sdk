using MirageSDK.Utils;
using MirageSDK.WalletConnectSharp.Unity;
using Nethereum.Web3;

namespace MirageSDK.Mobile
{
	public class MobileWeb3Provider
	{
		private readonly WalletConnect _walletConnect;

		public MobileWeb3Provider()
		{
			_walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
		}

		public IWeb3 CreateWeb3(string providerURI)
		{
			var session = _walletConnect.Session;
			var client =
				WalletConnectSharp.NEthereum.WalletConnectNEthereumExtensions.CreateProvider(session,
					new System.Uri(providerURI));
			var web3 = new Web3(client);
			return web3;
		}
	}
}