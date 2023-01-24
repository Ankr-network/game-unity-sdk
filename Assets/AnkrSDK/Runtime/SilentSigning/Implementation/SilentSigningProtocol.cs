using System.Threading.Tasks;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.SilentSigning.Data.Requests;
using AnkrSDK.SilentSigning.Data.Responses;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Core.Models;
using AnkrSDK.WalletConnectSharp.Unity;
using UnityEngine;

namespace AnkrSDK.SilentSigning
{
	public class SilentSigningProtocol : ISilentSigningHandler
	{
		private readonly WalletConnect _walletConnect;

		public ISilentSigningSessionHandler SessionHandler { get; }

		private bool _skipNextDeepLink;

		public SilentSigningProtocol()
		{
			_walletConnect = WalletConnectProvider.GetWalletConnect();
			SessionHandler = new SilentSigningSessionHandler();
			SetupDeeplinkOnEachMessage();
		}

		public async Task<string> RequestSilentSign(long timestamp, long chainId = 1)
		{
			var protocol = _walletConnect.Session;
			var data = new SilentSigningConnectionRequest(timestamp, chainId);
			var requestSilentSign =
				await protocol.Send<SilentSigningConnectionRequest, SilentSigningResponse>(data);
			if (!requestSilentSign.IsError)
			{
				SessionHandler.SaveSilentSession(requestSilentSign.Result);
			}

			return requestSilentSign.Result;
		}

		public Task DisconnectSilentSign()
		{
			var protocol = _walletConnect.Session;
			var secret = SessionHandler.GetSavedSessionSecret();
			var data = new SilentSigningDisconnectRequest(secret);
			SessionHandler.ClearSilentSession();
			return protocol.Send<SilentSigningDisconnectRequest, SilentSigningResponse>(data);
		}

		public async Task<string> SendSilentTransaction(string from, string to, string data = null, string value = null,
			string gas = null,
			string gasPrice = null, string nonce = null)
		{
			var transactionData = new SilentTransactionData
			{
				from = from,
				to = to,
				data = data,
				value = value,
				gas = gas,
				gasPrice = gasPrice,
				nonce = nonce,
				secret = SessionHandler.GetSavedSessionSecret()
			};
			var request = new SilentSigningTransactionRequest(transactionData);

			Debug.Log("[SS] SendSilentTransaction");
			SkipNextDeepLink();
			var response = await SendAndHandle(request);

			return response.Result;
		}

		public async Task<string> SilentSignMessage(string address, string message)
		{
			var request = new SilentSigningSignMessageRequest(address, message);
			SkipNextDeepLink();
			var response = await SendAndHandle(request);

			return response.Result;
		}

		public bool IsSilentSigningActive()
		{
			return SessionHandler.IsSessionSaved();
		}

		private void SkipNextDeepLink()
		{
			_skipNextDeepLink = true;
		}

		private void SetupDeeplinkOnEachMessage()
		{
			SessionHandler.SessionUpdated += OnSessionUpdated;
			var walletConnect = WalletConnectProvider.GetWalletConnect();
			walletConnect.SessionUpdated += OnSessionUpdated;
			if (walletConnect.Session != null)
			{
				SubscribeSession(walletConnect.Session);
			}
		}

		private void OnSessionUpdated()
		{
			var walletConnect = WalletConnectProvider.GetWalletConnect();
			SubscribeSession(walletConnect.Session);
		}

		private void SubscribeSession(WalletConnectSession session)
		{
			session.OnSend -= OnSessionSend;
			if (!IsSilentSigningActive())
			{
				session.OnSend += OnSessionSend;
			}
		}

		private void OnSessionSend(object sender, WalletConnectSession e)
		{
			if (_skipNextDeepLink)
			{
				_skipNextDeepLink = false;
				return;
			}

			var walletConnect = WalletConnectProvider.GetWalletConnect();
			walletConnect.OpenMobileWallet();
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

				SessionHandler.ClearSilentSession(); //Do we need to clear it each time?
			}

			return response;
		}
	}
}