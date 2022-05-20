using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.Web3;

namespace AnkrSDK.Core
{
	public class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IContractFunctions _contractFunctions;
		private readonly IDisconnectHandler _disconnectHandler;
		public INetworkHelper NetworkHelper { get; }
		public IEthHandler Eth { get; }

		public AnkrSDKWrapper(
			IContractFunctions contractFunctions,
			IEthHandler eth,
			IDisconnectHandler disconnectHandler,
			INetworkHelper networkHelper)
		{
			_contractFunctions = contractFunctions;
			_disconnectHandler = disconnectHandler;
			NetworkHelper = networkHelper;
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

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return _disconnectHandler.Disconnect(waitForNewSession);
		}
	}
}