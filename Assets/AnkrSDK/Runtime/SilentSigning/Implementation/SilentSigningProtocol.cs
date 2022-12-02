using System.Threading.Tasks;
using AnkrSDK.SilentSigning.Data.Requests;
using AnkrSDK.SilentSigning.Data.Responses;
using AnkrSDK.SilentSigning.Helpers;
using AnkrSDK.SilentSigning.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Unity;

namespace AnkrSDK.SilentSigning
{
	public class SilentSigningProtocol : ISilentSigningHandler
	{
		private readonly WalletConnect _walletConnect;

		public SilentSigningProtocol()
		{
			_walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
		}

		public async Task<SilentSigningResponse> RequestSilentSign(long timestamp)
		{
			var protocol = _walletConnect.Session;
			var data = new SilentSigningConnectionRequest(timestamp);
			var requestSilentSign =
				await protocol.Send<SilentSigningConnectionRequest, SilentSigningResponse>(data);
			if (!requestSilentSign.IsError)
			{
				SilentSigningSecretSaver.SaveSilentSession(requestSilentSign.Result);
			}

			return requestSilentSign;
		}

		public Task DisconnectSilentSign()
		{
			var protocol = _walletConnect.Session;
			var secret = SilentSigningSecretSaver.GetSavedSessionSecret();
			var data = new SilentSigningDisconnectRequest(secret);
			return protocol.Send<SilentSigningDisconnectRequest, SilentSigningResponse>(data);
		}

		public Task SendSilentTransaction(
			string from,
			string to,
			string data = null,
			string value = null,
			string gas = null,
			string gasPrice = null,
			string nonce = null
		)
		{
			var protocol = _walletConnect.Session;
			var transactionData = new TransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value != null ? AnkrSDKHelper.StringToBigInteger(value) : null,
				gas = gas != null ? AnkrSDKHelper.StringToBigInteger(gas) : null,
				gasPrice = gasPrice != null ? AnkrSDKHelper.StringToBigInteger(gasPrice) : null,
				nonce = nonce
			};

			var request = new SilentSigningTransactionRequest(transactionData);

			var response = protocol.Send<SilentSigningTransactionRequest, SilentSigningResponse>(request);
		}
	}
}