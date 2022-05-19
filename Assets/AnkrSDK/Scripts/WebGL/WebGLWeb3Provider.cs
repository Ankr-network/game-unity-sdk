using Nethereum.Web3;

namespace AnkrSDK.WebGL
{
	public static class WebGLWeb3Provider
	{
		public static IWeb3 CreateWeb3Provider(string providerURI)
		{
			return new Web3(providerURI);
		}
	}
}