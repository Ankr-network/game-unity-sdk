using System.Threading.Tasks;
using AnkrSDK.SilentSigning.Data.Requests;
using AnkrSDK.SilentSigning.Data.Responses;
using AnkrSDK.SilentSigning.Helpers;
using AnkrSDK.SilentSigning.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models;
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

		public bool IsSilentSigningActive()
		{
			return SilentSigningSecretSaver.IsSessionSaved();
		}

		public async Task<string> RequestSilentSign(long timestamp)
		{
			var protocol = _walletConnect.Session;
			var data = new SilentSigningConnectionRequest(timestamp);
			var requestSilentSign =
				await protocol.Send<SilentSigningConnectionRequest, SilentSigningResponse>(data);
			if (!requestSilentSign.IsError)
			{
				SilentSigningSecretSaver.SaveSilentSession(requestSilentSign.Result);
			}

			return requestSilentSign.Result;
		}

		public Task DisconnectSilentSign()
		{
			var protocol = _walletConnect.Session;
			var secret = SilentSigningSecretSaver.GetSavedSessionSecret();
			var data = new SilentSigningDisconnectRequest(secret);
			SilentSigningSecretSaver.ClearSilentSession();
			return protocol.Send<SilentSigningDisconnectRequest, SilentSigningResponse>(data);
		}

		public async Task<string> SendSilentTransaction(TransactionData transaction)
		{
			var transactionData = new SilentTransactionData
			{
				from = transaction.from,
				to = transaction.to,
				data = transaction.data,
				value = transaction.value,
				gas = transaction.gas,
				gasPrice = transaction.gasPrice,
				nonce = transaction.nonce,
				secret = SilentSigningSecretSaver.GetSavedSessionSecret()
			};
			var request = new SilentSigningTransactionRequest(transactionData);
			
			var response = await SendAndHandle(request);

			return response.Result;
		}

		public async Task<string> MakeSilentSignMessage(string address, string message)
		{
			var request = new SilentSigningSignMessageRequest(address, message);
			
			var response = await SendAndHandle(request);

			return response.Result;
		}

		private async Task<SilentSigningResponse> SendAndHandle<TRequest>(TRequest request) where TRequest : JsonRpcRequest
		{
			var protocol = _walletConnect.Session;
			var response = await protocol.Send<TRequest, SilentSigningResponse>(request);
			if (response.IsError)
			{
				if (response.Error.Message == "Session expired") //Todo replace with code comparison
				{
					SilentSigningSecretSaver.ClearSilentSession();
				}
			}

			return response;
		}
	}
}