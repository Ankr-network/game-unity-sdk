using System.Collections.Generic;
using AnkrSDK.MirageAPI.MirageID.Data.CreateUser;
using AnkrSDK.MirageAPI.MirageID.Helpers;
using AnkrSDK.MirageAPI.MirageID.Infrastructure;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.MirageAPI.MirageID.Implementation
{
	public class MirageIdApplicationRequests : IMirageIdApplicationRequests
	{
		private static MirageIdEnv _env;
		private string _clientId;
		private string _clientSecret;
		private string _applicationToken;

		public MirageIdApplicationRequests()
		{
			_env = new MirageIdEnv();
		}
		public MirageIdApplicationRequests(MirageIdEnv env)
		{
			_env = env;
		}

		public bool IsInitialized()
		{
			return !string.IsNullOrEmpty(_applicationToken)
			       && !string.IsNullOrEmpty(_clientId)
			       && !string.IsNullOrEmpty(_clientSecret);
		}

		public async UniTask<string> Initialize(string clientId, string clientSecret)
		{
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
					_env.CreateUserURL, payload,
					headers);
			return answer?.WalletId;
		}
	}
}