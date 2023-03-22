using System.Collections.Generic;
using MirageSDK.MirageAPI.MirageID.Data.Token;
using MirageSDK.Utils;
using Cysharp.Threading.Tasks;

namespace MirageSDK.MirageAPI.MirageID.Helpers
{
	public static class MirageIdRequestsHelper
	{
		public static async UniTask<string> GetTokenRequest(Dictionary<string, string> payload)
		{
			var answer =
				await WebHelper.SendPostRequestURLEncoded<TokenResponseDTO>(MirageEnvironment.TokenURL, payload);
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
			return WebHelper.SendPostRequestURLEncoded(MirageEnvironment.LogoutURL, payload,
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