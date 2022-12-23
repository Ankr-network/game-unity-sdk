using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;

namespace AnkrSDK.Core
{
	public class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IContractFunctions _contractFunctions;
		public IWalletHandler WalletHandler { get; }
		public INetworkHelper NetworkHelper { get; }
		public IEthHandler Eth { get; }
		public ISilentSigningHandler SilentSigningHandler { get; }

		public AnkrSDKWrapper(
			IContractFunctions contractFunctions,
			IEthHandler eth,
			IWalletHandler disconnectHandler,
			INetworkHelper networkHelper,
			ISilentSigningHandler silentSigningHandler = null
		)
		{
			_contractFunctions = contractFunctions;
			WalletHandler = disconnectHandler;
			NetworkHelper = networkHelper;
			SilentSigningHandler = silentSigningHandler;
			Eth = eth;
		}

		public IContract GetContract(string contractAddress, string contractABI)
		{
			return new Contract(Eth, _contractFunctions, contractAddress, contractABI);
		}

		public IContractEventSubscriber CreateSubscriber(string wsUrl)
		{
			return new ContractEventSubscriber(wsUrl);
		}
	}
}