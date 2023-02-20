using System.Collections.Generic;
using AnkrSDK.MirageAPI.Data;
using AnkrSDK.MirageAPI.MirageID.Data.CreateUser;
using AnkrSDK.MirageAPI.MirageID.Helpers;
using AnkrSDK.MirageAPI.MirageID.Infrastructure;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.MirageAPI.MirageID.Implementation
{
	public class MirageIdApplicationRequests : IMirageIdApplicationRequests
	{
		private string _clientId;
		private string _clientSecret;
		private string _applicationToken;

		public bool IsInitialized()
		{
			return !string.IsNullOrEmpty(_applicationToken)
			       && !string.IsNullOrEmpty(_clientId)
			       && !string.IsNullOrEmpty(_clientSecret);
		}

		public UniTask<string> Initialize(MirageAPISettingsSO mirageAPISettingsSO)
		{
			var clientID = mirageAPISettingsSO.ClientID;
			var clientSecret = mirageAPISettingsSO.ClientSecret;
			return InitializeInternal(clientID, clientSecret);
		}

		public UniTask<string> Initialize(string clientId, string clientSecret)
		{
			return InitializeInternal(clientId, clientSecret);
		}

		public UniTask Logout()
		{
			var token = _applicationToken;
			_applicationToken = string.Empty;
			return MirageIdRequestsHelper.LogoutRequest(_clientId, token);
		}

		public async UniTask<string> CreateUser(
			string email,
			string username,
			string firstName = null,
			string lastName = null)
		{
			var payload = new CreateUserRequestDTO
			{
				EMail = email,
				UserName = username,
				FirstName = firstName,
				LastName = lastName
			};

			var headers = MirageIdRequestsHelper.GetAuthorizationHeader(_applicationToken);
			var answer =
				await WebHelper.SendPostRequest<CreateUserRequestDTO, CreateUserResponseDTO>(
					MirageEnvironment.CreateUserURL, payload,
					headers);
			return answer?.WalletId;
		}

		private async UniTask<string> InitializeInternal(string clientId, string clientSecret)
		{
			if (string.IsNullOrEmpty(clientId))
			{
				Debug.LogError("Invalid clientId");
				return string.Empty;
			}

			if (string.IsNullOrEmpty(clientSecret))
			{
				Debug.LogError("Invalid clientSecret");
				return string.Empty;
			}

			_clientId = clientId;
			_clientSecret = clientSecret;

			var payload = new Dictionary<string, string>
			{
				{ "client_secret", _clientSecret },
				{ "client_id", _clientId },
				{ "grant_type", "client_credentials" },
			};

			var token = await MirageIdRequestsHelper.GetTokenRequest(payload);
			_applicationToken = token;

			return _applicationToken;
		}
	}
}