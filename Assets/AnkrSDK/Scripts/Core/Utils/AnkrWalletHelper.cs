using System.Threading.Tasks;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Unity;

namespace AnkrSDK.Core.Utils
{
	public static class AnkrWalletHelper
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
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
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