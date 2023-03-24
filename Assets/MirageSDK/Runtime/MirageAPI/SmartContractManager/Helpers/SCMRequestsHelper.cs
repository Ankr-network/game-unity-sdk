using System.Collections.Generic;

namespace MirageSDK.MirageAPI.SmartContractManager.Helpers
{
	public static class SCMRequestsHelper
	{
		public static Dictionary<string, string> GetAuthorizationHeader(string token)
		{
			return new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};
		}
	}
}