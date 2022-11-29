namespace MirageSDK.Core.Infrastructure
{
	public interface IMirageSDK
	{
		IEthHandler Eth { get; }
		INetworkHelper NetworkHelper { get; }
		IWalletHandler WalletHandler { get; }
		IContract GetContract(string contractAddress, string contractABI);
		IContractEventSubscriber CreateSubscriber(string wsUrl);
	}
}
