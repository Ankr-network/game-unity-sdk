namespace AnkrSDK.MirageAPI.SmartContractManager.Helpers
{
	public static class SCMEndpoints
	{
		private const string BaseUrl = "https://mosaic-game-services.staging.mirage.xyz/api/";

		private const string GameProjectClientEndpoint = "GameProjectClient/";
		private const string ContractEndpoint = "contract";

		public static string GetContractInfoURL(string contractId) =>
			$"{BaseUrl}{GameProjectClientEndpoint}{ContractEndpoint}/{contractId}";

		public static string GetAllContractsURL => $"{BaseUrl}{GameProjectClientEndpoint}{ContractEndpoint}";
	}
}