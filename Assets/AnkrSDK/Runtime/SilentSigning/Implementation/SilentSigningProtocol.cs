using System.Threading.Tasks;
using AnkrSDK.SilentSigning.Data.Requests;
using AnkrSDK.SilentSigning.Data.Responses;
using AnkrSDK.SilentSigning.Helpers;
using AnkrSDK.SilentSigning.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;

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

			Debug.Log("[SS] SendSilentTransaction");
			var response = await SendAndHandle(request);

			return response.Result;
		}

		public async Task<string> SilentSignMessage(string address, string message)
		{
			var request = new SilentSigningSignMessageRequest(address, message);

			var response = await SendAndHandle(request);

			return response.Result;
		}

		private async Task<SilentSigningResponse> SendAndHandle<TRequest>(TRequest request)
			where TRequest : JsonRpcRequest
		{
			var protocol = _walletConnect.Session;
			var response = await protocol.Send<TRequest, SilentSigningResponse>(request);
			if (response.IsError)
			{
				switch (response.Error.Code)
				{
					case -30000:
					{
						Debug.LogError("[SS] Invalid secret token");
						break;
					}
					case -30001:
					{
						Debug.LogError("[SS] Session expired");
						break;
					}
					case -30002:
					{
						Debug.LogError("[SS] Session already connected");
						break;
					}
					case -30003:
					{
						Debug.LogError("[SS] Incorrect \"until\" field");
						break;
					}
					case -30004:
					{
						Debug.LogError("[SS] Chain id not supported");
						break;
					}
					case -30005:
					{
						Debug.LogError("[SS] Silent Sign session is not connected");
						break;
					}
					case -30006:
					{
						Debug.LogError("[SS] SilentSigning: Invalid DApp name");
						break;
					}
					case -30007:
					{
						Debug.LogError("[SS] Silent Sign Session already connected");
						break;
					}
				}

				SilentSigningSecretSaver.ClearSilentSession(); //Do we need to clear it each time?
			}

			return response;
		}
	}
}