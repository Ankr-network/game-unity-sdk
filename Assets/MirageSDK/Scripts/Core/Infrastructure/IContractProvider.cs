using Nethereum.Web3;

namespace MirageSDK.Core.Infrastructure
{
	public interface IContractProvider
	{
		IContract GetContract(string contractAddress, string contractABI, string providerURI);
		IContract GetContract(IWeb3 web3, string contractAddress, string contractABI);
	}
}