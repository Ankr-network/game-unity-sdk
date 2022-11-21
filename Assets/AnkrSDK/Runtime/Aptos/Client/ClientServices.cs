using AnkrSDK.Aptos.DTO;
using AnkrSDK.Aptos.Services;

namespace AnkrSDK.Aptos
{
	public class ClientServices
	{
		private OpenAPIConfig _config;
		
		public readonly TransactionsService TransactionsService;
		public readonly GeneralService GeneralService;
		public readonly AccountsService AccountsService;

		protected static OpenAPIConfig EnhanceConfig(OpenAPIConfig config)
		{
			return new OpenAPIConfig
			{
				Base = config.Base ?? "https://fullnode.devnet.aptoslabs.com",
				Version = config.Version ?? "v1",
				WithCredentials = config.WithCredentials ?? false,
				Credentials = config.Credentials ?? "include",
				Token = config.Token,
				Username = config.Username,
				Password = config.Password,
				Headers = config.Headers
			};
		}
		
		public ClientServices(OpenAPIConfig config)
		{
			config = EnhanceConfig(config);
			TransactionsService = new TransactionsService(config);
			GeneralService = new GeneralService(config);
			AccountsService = new AccountsService(config);
		}
	}
}