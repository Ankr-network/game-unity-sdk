using System.Threading.Tasks;
using WalletConnectSharp.Core.Models.Ethereum;
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