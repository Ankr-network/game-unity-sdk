using System.Collections.Generic;
using AnkrSDK.MirageAPI.MirageID.Data;

namespace AnkrSDK.MirageAPI.MirageID.Helpers
{
	public static class EnvironmentSetting
	{
		private static readonly Dictionary<EnvironmentType, EnvSetting> Environments = CreateEnvironments();

		public static EnvSetting CurrentEnv;
		
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
			CurrentEnv = envSettings;
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