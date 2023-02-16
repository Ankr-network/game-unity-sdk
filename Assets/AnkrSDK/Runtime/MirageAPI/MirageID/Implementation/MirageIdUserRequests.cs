using System.Collections.Generic;
using AnkrSDK.MirageAPI.MirageID.Data.Wallet;
using AnkrSDK.MirageAPI.MirageID.Helpers;
using AnkrSDK.MirageAPI.MirageID.Infrastructure;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.MirageAPI.MirageID.Implementation
{
	public class MirageIdUserRequests : IMirageIdUserRequests
	{
		private string _clientId;
		private string _clientSecret;
		private string _userToken;

		public bool IsAuthorized()
		{
			return !string.IsNullOrEmpty(_userToken)
			       && !string.IsNullOrEmpty(_clientId)
			       && !string.IsNullOrEmpty(_clientSecret);
		}

		public async UniTask<string> SignIn(string clientId, string clientSecret, string username, string password)
		{
			_clientId = clientId;
			_clientSecret = clientSecret;
			_userToken = await SignIn(username, password);
			return _userToken;
		}

		public UniTask Logout()
		{
			var token = _userToken;
			_userToken = string.Empty;
			return MirageIdRequestsHelper.LogoutRequest(_clientId, token);
		}

		public UniTask<WalletInfoResponseDTO> WalletInfo()
		{
			var headers = MirageIdRequestsHelper.GetAuthorizationHeader(_userToken);

			return WebHelper.SendGetRequest<WalletInfoResponseDTO>(
				MirageEnvironment.WalletInfoURL,
				headers);
		}

		private UniTask<string> SignIn(string username, string password)
		{
			var payload = new Dictionary<string, string>
			{
				{ "client_secret", _clientSecret },
				{ "client_id", _clientId },
				{ "grant_type", "password" },
				{ "username", username },
				{ "password", password }
			};

			return MirageIdRequestsHelper.GetTokenRequest(payload);
		}
	}
}