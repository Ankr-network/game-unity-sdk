using AnkrSDK.Aptos.DTO;

namespace AnkrSDK.Aptos.Services
{
	public class BaseService
	{
		protected string URL;
		public BaseService(OpenAPIConfig config)
		{
			URL = $"{config.Base}/{config.Version}";
		}
	}
}