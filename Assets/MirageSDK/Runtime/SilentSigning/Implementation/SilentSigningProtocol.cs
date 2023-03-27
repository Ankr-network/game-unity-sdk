using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.SilentSigning.Data.Requests;
using MirageSDK.SilentSigning.Data.Responses;
using MirageSDK.Utils;
using MirageSDK.WalletConnect.VersionShared.Models;
using MirageSDK.WalletConnectSharp.Unity.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MirageSDK.SilentSigning.Implementation
{
	public class SilentSigningProtocol : ISilentSigningHandler
	{
		public ISilentSigningSessionHandler SessionHandler { get; }
		private WalletConnectSharp.Unity.WalletConnect WalletConnect => ConnectProvider<WalletConnectSharp.Unity.WalletConnect>.GetConnect();

		private bool _skipNextDeepLink;

		public SilentSigningProtocol()
		{
			SessionHandler = new SilentSigningSessionHandler();
			SetupDeeplinkOnEachMessage();
		}

		public async UniTask<string> RequestSilentSign(long timestamp, long chainId = 1)
		{
			var data = new SilentSigningConnectionRequest(timestamp, chainId);
			var requestSilentSign =
				await WalletConnect.Send<SilentSigningConnectionRequest, SilentSigningResponse>(data);

			Debug.Log($"ANTON DEBUG: RequestSilentSign error found: {requestSilentSign.IsError}");
			if (!requestSilentSign.IsError)
			{
				SessionHandler.SaveSilentSession(requestSilentSign.Result);
			}

			return requestSilentSign.Result;
		}

		public UniTask DisconnectSilentSign()
		{
			var secret = SessionHandler.GetSavedSessionSecret();
			var data = new SilentSigningDisconnectRequest(secret);
			SessionHandler.ClearSilentSession();
			return WalletConnect.Send<SilentSigningDisconnectRequest, SilentSigningResponse>(data);
		}

		public async UniTask<string> SendSilentTransaction(string from, string to, string data = null, string value = null,
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
			Debug.Log("ANTON DEBUG: SendSilentTransaction");
			SkipNextDeepLink();
			var response = await SendAndHandle(request);

			return response.Result;
		}

		public async UniTask<string> SilentSignMessage(string address, string message)
		{
			var request = new SilentSigningSignMessageRequest(address, message);
			Debug.Log("ANTON DEBUG: SilentSignMessage");
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
			Debug.Log("ANTON DEBUG: SilentSigningProtocol skip next deep link called");
			_skipNextDeepLink = true;
		}

		private void SetupDeeplinkOnEachMessage()
		{
			SessionHandler.SessionUpdated += OnSessionUpdated;
			WalletConnect.SessionStatusUpdated += OnSessionStatusUpdated;
			SubscribeSession();
		}

		private void OnSessionStatusUpdated(WalletConnectTransitionBase walletConnectTransitionBase)
		{
			SubscribeSession();
		}

		private void OnSessionUpdated()
		{
			SubscribeSession();
		}

		private void SubscribeSession()
		{
			var stackTrace = StackTraceUtility.ExtractStackTrace();
			Debug.Log("ANTON DEBUG: SilentSigningProtocol OnSessionSend unsubscribed " + stackTrace);
			WalletConnect.OnSend -= OnSessionSend;
			if (!IsSilentSigningActive())
			{
				Debug.Log("ANTON DEBUG: SilentSigningProtocol OnSessionSend subscribed " + stackTrace);
				WalletConnect.OnSend += OnSessionSend;
			}
		}

		private void OnSessionSend()
		{
			Debug.Log("ANTON DEBUG: SilentSigningProtocol OnSessionSend");
			if (_skipNextDeepLink)
			{
				Debug.Log("ANTON DEBUG: SilentSigningProtocol deep link skipped");
				_skipNextDeepLink = false;
				return;
			}

			WalletConnect.OpenMobileWallet();
		}

		private async UniTask<SilentSigningResponse> SendAndHandle<TRequest>(TRequest request)
			where TRequest : JsonRpcRequest
		{
			var response = await WalletConnect.Send<TRequest, SilentSigningResponse>(request);
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