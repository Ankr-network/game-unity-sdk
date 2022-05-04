using AnkrSDK.Core.Implementation;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IAnkrSDK
	{
		IEthHandler Eth { get; }
		IContract GetContract(string contractAddress, string contractABI);
		ContractEventSubscriber CreateSubscriber(string wsUrl);
	}
}
