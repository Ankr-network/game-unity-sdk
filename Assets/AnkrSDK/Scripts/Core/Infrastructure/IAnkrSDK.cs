namespace AnkrSDK.Core.Infrastructure
{
	public interface IAnkrSDK
	{
		IEthHandler Eth { get; }
		IContract GetContract(string contractAddress, string contractABI);
		IContractEventSubscriber CreateSubscriber(string wsUrl);
	}
}
