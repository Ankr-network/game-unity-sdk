namespace AnkrSDK.MirageAPI.MirageID.Helpers
{
	public static class MirageIdEndpoints
	{
		private const string BaseUrl = "https://mirage-id.staging.mirage.xyz/api/";

		private const string CreateUserEndpoint = "Users";
		private const string WalletEndpoint = "Wallet";

		public const string TokenEndpoint =
			"https://auth.staging.mirage.xyz/realms/mirage-id/protocol/openid-connect/token";
		public const string LogoutEndpoint =
			"https://auth.staging.mirage.xyz/auth/realms/mirage-id/protocol/openid-connect/logout";

		public static string CreateUserURL => BaseUrl + CreateUserEndpoint;
		public static string WalletInfoURL => BaseUrl + WalletEndpoint;
	}
}