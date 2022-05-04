using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;

namespace AnkrSDK.Core
{
	public static class AnkrSDKFactory
	{
		public static IAnkrSDK GetAnkrSDKInstance(string providerURI)
		{
			return new AnkrSDKWrapper(providerURI);
		}
	}
}