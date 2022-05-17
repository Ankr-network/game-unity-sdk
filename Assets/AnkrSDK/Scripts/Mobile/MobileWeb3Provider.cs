using AnkrSDK.Core.Infrastructure;
using Nethereum.Web3;

namespace AnkrSDK.Mobile
{
	public class MobileWeb3Provider : IWeb3Provider
	{
		public IWeb3 CreateWeb3(string providerURI)
		{
			var wcProtocol = WalletConnectSharp.Unity.WalletConnect.ActiveSession;
			var client =
				WalletConnectSharp.NEthereum.WalletConnectNEthereumExtensions.CreateProvider(wcProtocol,
					new System.Uri(providerURI));
			var web3 = new Web3(client);
			return web3;
		}
	}
}