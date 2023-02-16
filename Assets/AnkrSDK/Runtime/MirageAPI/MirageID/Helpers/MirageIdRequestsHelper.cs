using System.Collections.Generic;
using AnkrSDK.MirageAPI.MirageID.Data.Token;
using AnkrSDK.MirageAPI.MirageID.Implementation;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.MirageAPI.MirageID.Helpers
{
	public static class MirageIdRequestsHelper
	{
		private static IMirageIdEnv _env;
		
		static MirageIdRequestsHelper()
		{
			_env = new MirageIdEnv();
		}

		static void SetEnvironment(IMirageIdEnv env)
		{
			_env = env;
		}
		
		public static async UniTask<string> GetTokenRequest(Dictionary<string, string> payload)
		{
			var answer =
				await WebHelper.SendPostRequestURLEncoded<TokenResponseDTO>(_env.TokenEndpoint, payload);
			var answerToken = answer?.AccessToken;
			return answerToken;
		}

		public static UniTask LogoutRequest(string clientId, string token)
		{
			var payload = new Dictionary<string, string>
			{
				{ "client_id", clientId }
			};

			var authHeader = GetAuthorizationHeader(token);
			return WebHelper.SendPostRequestURLEncoded(_env.LogoutEndpoint, payload,
				authHeader);
		}

		public static Dictionary<string, string> GetAuthorizationHeader(string token)
		{
			return new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};
		}
	}
}