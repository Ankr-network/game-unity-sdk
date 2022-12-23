namespace AnkrSDK.Core.Infrastructure
{
	public interface IAnkrSDK
	{
		IEthHandler Eth { get; }
		INetworkHelper NetworkHelper { get; }
		IWalletHandler WalletHandler { get; }
		IContract GetContract(string contractAddress, string contractABI);
		IContractEventSubscriber CreateSubscriber(string wsUrl);
		ISilentSigningHandler SilentSigningHandler { get; }
	}
}
