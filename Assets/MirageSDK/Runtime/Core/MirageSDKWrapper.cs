using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;

namespace MirageSDK.Core
{
	public class MirageSDKWrapper : IMirageSDK
	{
		private readonly IContractFunctions _contractFunctions;
		public IWalletHandler WalletHandler { get; }
		public INetworkHelper NetworkHelper { get; }
		public IEthHandler Eth { get; }
		public ISilentSigningHandler SilentSigningHandler { get; }

		public MirageSDKWrapper(
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