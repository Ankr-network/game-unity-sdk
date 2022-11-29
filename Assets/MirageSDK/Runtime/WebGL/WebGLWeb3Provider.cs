using Nethereum.Web3;

namespace MirageSDK.WebGL
{
	public class WebGLWeb3Provider
	{
		public IWeb3 CreateWeb3(string providerURI)
		{
			return new Web3(providerURI);
		}
	}
}