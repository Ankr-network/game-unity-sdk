using System.Collections.Generic;

namespace AnkrSDK.MirageAPI.MirageID.Helpers
{
	public static class MirageEnvironment
	{
		private const string GetTokenEndpoint = "/realms/mirage-id/protocol/openid-connect/token";
		private const string LogoutEndpoint = "/realms/mirage-id/protocol/openid-connect/token";
		private const string CreateUserEndpoint = "/Users";
		private const string WalletEndpoint = "/Wallet";
		private const string NftTransactionEndpoint = "/NftTransaction";
		private const string NFTTransactionDataEndpoint = "/GetTransactionData";
		private const string TransferNFTEndpoint = "/TransferNft";

		public static string TokenURL => EnvironmentSetting.CurrentEnv.AuthUrl + GetTokenEndpoint;
		public static string LogoutURL => EnvironmentSetting.CurrentEnv.AuthUrl + LogoutEndpoint;
		public static string CreateUserURL => EnvironmentSetting.CurrentEnv.MirageIdUrl + CreateUserEndpoint;
		public static string WalletInfoURL => EnvironmentSetting.CurrentEnv.MirageIdUrl + WalletEndpoint;
		public static string TransferNFTURL => EnvironmentSetting.CurrentEnv.MirageIdUrl + NftTransactionEndpoint + TransferNFTEndpoint;
		public static string NFTTransactionDataURL => EnvironmentSetting.CurrentEnv.MirageIdUrl + NftTransactionEndpoint + NFTTransactionDataEndpoint;
		public static string CreateNFT => EnvironmentSetting.CurrentEnv.MirageIdUrl + NftTransactionEndpoint;

		static MirageEnvironment()
		{
			EnvironmentSetting.UseStagingEnv();
		}
	}
}