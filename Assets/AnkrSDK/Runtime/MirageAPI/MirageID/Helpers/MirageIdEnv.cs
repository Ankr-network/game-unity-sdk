using System.Collections.Generic;
using AnkrSDK.MirageAPI.MirageID.Implementation;

namespace AnkrSDK.MirageAPI.MirageID.Helpers
{
	public class MirageIdEnv : IMirageIdEnv
	{
		private static readonly Dictionary<Environments, EnvSetting> Environments = CreateEnvironments();

		private string _currentEnvMirageId;
		private string _currentEnvAuth;

		private const string CreateUserEndpoint = "/Users";
		private const string WalletEndpoint = "/Wallet";

		public string TokenEndpoint => _currentEnvAuth + "/realms/mirage-id/protocol/openid-connect/token";
		public string LogoutEndpoint => _currentEnvAuth + "/auth/realms/mirage-id/protocol/openid-connect/logout";
		public string CreateUserURL => _currentEnvMirageId + CreateUserEndpoint;
		public string WalletInfoURL => _currentEnvMirageId + WalletEndpoint;

		public MirageIdEnv()
		{
			UseStagingEnv();
		}
		
		public  void UseStagingEnv()
		{
			ChangeEnvironment(Environments[Helpers.Environments.Staging]);
		}
		
		public  void UseProductionEnv()
		{
			ChangeEnvironment(Environments[Helpers.Environments.Production]);
		}

		public  void ChangeEnvironment(EnvSetting envSettings)
		{
			_currentEnvAuth = envSettings.AuthUrl;
			_currentEnvMirageId = envSettings.MirageIdUrl;
		}
		
		private static Dictionary<Environments, EnvSetting> CreateEnvironments()
		{
			return new Dictionary<Environments, EnvSetting>
			{
				{
					Helpers.Environments.Staging,
					new EnvSetting
					{
						MirageIdUrl = "https://mirage-id.staging.mirage.xyz/api",
						AuthUrl = "https://auth.staging.mirage.xyz"
					}
				},
				{
					Helpers.Environments.Production,
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