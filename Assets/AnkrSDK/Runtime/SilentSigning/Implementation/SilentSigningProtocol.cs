using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnect2.Events;
using AnkrSDK.WalletConnect2.RpcRequests;
using AnkrSDK.WalletConnect2.RpcRequests.SilentSigning;
using AnkrSDK.WalletConnect2.RpcResponses.SilentSigning;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.SilentSigning.Implementation
{
	public class SilentSigningProtocol : ISilentSigningHandler
	{
		public ISilentSigningSessionHandler SessionHandler { get; }
		private AnkrSDK.WalletConnect2.WalletConnect2 WalletConnect => ConnectProvider<AnkrSDK.WalletConnect2.WalletConnect2>.GetConnect();

		private bool _skipNextDeepLink;

		public SilentSigningProtocol()
		{
			SessionHandler = new SilentSigningSessionHandler();
			SetupDeeplinkOnEachMessage();
		}

		public async UniTask<string> RequestSilentSign(long timestamp, long chainId = 1)
		{
			var data = new SilentSigningConnectionRequestData(timestamp, chainId);
			var requestSilentSign =
				await WalletConnect.Send<SilentSigningConnectionRequestData, SilentSigningResponseData>(data);
			if (!requestSilentSign.IsError)
			{
				SessionHandler.SaveSilentSession(requestSilentSign.Result);
			}

			return requestSilentSign.Result;
		}

		public UniTask DisconnectSilentSign()
		{
			var secret = SessionHandler.GetSavedSessionSecret();
			var data = new SilentSigningDisconnectRequestData(secret);
			SessionHandler.ClearSilentSession();
			return WalletConnect.Send<SilentSigningDisconnectRequestData, SilentSigningResponseData>(data);
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
			var request = new SilentSigningTransactionRequestData(transactionData);

			Debug.Log("[SS] SendSilentTransaction");
			SkipNextDeepLink();
			var response = await SendAndHandle(request);

			return response.Result;
		}

		public async UniTask<string> SilentSignMessage(string address, string message)
		{
			var request = new SilentSigningSignMessageRequestData(address, message);
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
			WalletConnect.SessionStatusUpdated += OnSessionStatusUpdated;
			SubscribeSession();
		}

		private void OnSessionStatusUpdated(WalletConnect2TransitionBase walletConnectTransitionBase)
		{
			SubscribeSession();
		}

		private void OnSessionUpdated()
		{
			SubscribeSession();
		}

		private void SubscribeSession()
		{
			WalletConnect.OnSend -= OnSessionSend;
			if (!IsSilentSigningActive())
			{
				WalletConnect.OnSend += OnSessionSend;
			}
		}

		private void OnSessionSend()
		{
			if (_skipNextDeepLink)
			{
				_skipNextDeepLink = false;
				return;
			}

			WalletConnect.OpenMobileWallet();
		}

		private async UniTask<SilentSigningResponseData> SendAndHandle<TRequest>(TRequest request)
			where TRequest : RpcRequestListDataBase
		{
			var response = await WalletConnect.Send<TRequest, SilentSigningResponseData>(request);
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