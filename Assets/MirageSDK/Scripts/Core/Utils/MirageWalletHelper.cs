using System.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core.Models.Ethereum;
using MirageSDK.WalletConnectSharp.Unity;

namespace MirageSDK.Core.Utils
{
	public static class MirageWalletHelper
	{
		public static Task<string> SendTransaction(
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var address = WalletConnect.ActiveSession.Accounts[0];

			var transactionData = new TransactionData
			{
				from = address,
				to = to,
				data = data,
				value = value != null ? MirageSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? MirageSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? MirageSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			return SendTransaction(transactionData);
		}

		private static Task<string> SendTransaction(TransactionData data)
		{
			return WalletConnect.ActiveSession.EthSendTransaction(data);
		}
	}
}