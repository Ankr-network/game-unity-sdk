using System.Threading.Tasks;
using MirageSDK.Plugins.WalletConnectSharp.Core.Models.Ethereum;
using MirageSDK.Plugins.WalletConnectSharp.Unity;
using WalletConnectSharp.Unity;

namespace MirageSDK.Core.Utils
{
	public static class WalletConnectExtension
	{
		public static async Task<string> SendTransaction(this TransactionData data)
		{
			return await WalletConnect.ActiveSession.EthSendTransaction(data);
		}
	}
}