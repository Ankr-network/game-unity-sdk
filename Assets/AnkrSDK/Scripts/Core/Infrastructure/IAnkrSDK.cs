using AnkrSDK.Core.Implementation;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IAnkrSDK
	{
		EthHandler Eth { get; }
		IContract GetContract(string contractAddress, string contractABI);
	}
}
