using MirageSDK.Core.Implementation;

namespace MirageSDK.Core.Infrastructure
{
	public interface IMirageSDK
	{
		EthHandler Eth { get; }
		IContract GetContract(string contractAddress, string contractABI);
	}
}
