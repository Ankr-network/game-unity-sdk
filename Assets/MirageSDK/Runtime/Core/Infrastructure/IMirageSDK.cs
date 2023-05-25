using MirageSDK.Data;

namespace MirageSDK.Core.Infrastructure
{
	public interface IMirageSDK
	{
		IEthHandler Eth { get; }
		IContract GetContract(string contractAddress, string contractABI);
		IContract GetContract(ContractInformationSO contractInfo);
		IContractEventSubscriber CreateSubscriber(string wsUrl);
		ISilentSigningHandler SilentSigningHandler { get; }
	}
}
