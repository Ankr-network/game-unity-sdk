using System.Collections.Generic;

namespace AnkrSDK.MirageAPI.MirageID.Helpers
{
	public static class MirageEnvironment
	{
		private static readonly Dictionary<EnvironmentType, EnvSetting> Environments = CreateEnvironments();

		private static EnvSetting _currentEnv;

		private const string CreateUserEndpoint = "/Users";
		private const string WalletEndpoint = "/Wallet";
		private const string NftTransactionEndpoint = "/NftTransaction";
		private const string NFTTransactionDataEndpoint = "/GetTransactionData";
		private const string TransferNFTEndpoint = "/TransferNft";

		public static string TokenEndpoint => _currentEnv.AuthUrl + "/realms/mirage-id/protocol/openid-connect/token";
		public static string LogoutEndpoint => _currentEnv.AuthUrl + "/auth/realms/mirage-id/protocol/openid-connect/logout";
		public static string CreateUserURL => _currentEnv.MirageIdUrl + CreateUserEndpoint;
		public static string WalletInfoURL => _currentEnv.MirageIdUrl + WalletEndpoint;
		public static string TransferNFTURL =>  _currentEnv.MirageIdUrl + NftTransactionEndpoint + TransferNFTEndpoint;
		public static string NFTTransactionDataURL =>  _currentEnv.MirageIdUrl + NftTransactionEndpoint + NFTTransactionDataEndpoint;
		public static string CreateNFT =>  _currentEnv.MirageIdUrl + "NftTransaction";

		static MirageEnvironment()
		{
			UseStagingEnv();
		}

		public static void UseStagingEnv()
		{
			ChangeEnvironment(Environments[EnvironmentType.Staging]);
		}

		public static void UseProductionEnv()
		{
			ChangeEnvironment(Environments[EnvironmentType.Production]);
		}

		public static void ChangeEnvironment(EnvSetting envSettings)
		{
			_currentEnv = envSettings;
		}

		private static Dictionary<EnvironmentType, EnvSetting> CreateEnvironments()
		{
			return new Dictionary<EnvironmentType, EnvSetting>
			{
				{
					EnvironmentType.Staging,
					new EnvSetting
					{
						MirageIdUrl = "https://mirage-id.staging.mirage.xyz/api",
						AuthUrl = "https://auth.staging.mirage.xyz"
					}
				},
				{
					EnvironmentType.Production,
					new EnvSetting
					{
						MirageIdUrl = "https://mirage-id.mirage.xyz",
						AuthUrl = "https://auth.production.mirage.xyz"
					}
				}
			};
		}
	}
}