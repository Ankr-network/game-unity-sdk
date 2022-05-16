using Nethereum.Web3;

namespace AnkrSDK.Mobile
{
	public static class MobileWeb3Provider
	{
		public static IWeb3 CreateWeb3Provider(string providerURI)
		{
			var wcProtocol = WalletConnectSharp.Unity.WalletConnect.ActiveSession;
			var client =
				WalletConnectSharp.NEthereum.WalletConnectNEthereumExtensions.CreateProvider(wcProtocol,
					new System.Uri(providerURI));
			var web3Provider = new Web3(client);
			return web3Provider;
		}
	}
}