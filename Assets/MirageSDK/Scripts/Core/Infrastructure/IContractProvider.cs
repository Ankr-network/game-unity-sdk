using Nethereum.Web3;

namespace MirageSDK.Core.Infrastructure
{
	public interface IContractProvider
	{
		IContract GetContract(string contractAddress, string contractABI);
		IContract GetContract(string providerURI, string contractAddress, string contractABI);
		IContract GetContract(IWeb3 web3, string contractAddress, string contractABI);
	}
}