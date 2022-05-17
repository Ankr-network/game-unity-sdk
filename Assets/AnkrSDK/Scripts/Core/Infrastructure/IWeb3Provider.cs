using Nethereum.Web3;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IWeb3Provider
	{
		IWeb3 CreateWeb3(string providerURI);
	}
}