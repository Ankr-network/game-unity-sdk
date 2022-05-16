using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Nethereum.Web3;

namespace AnkrSDK.Core
{
	public class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IWeb3 _web3Provider;
		private readonly IContractFunctions _contractFunctions;
		private readonly IDisconnectHandler _disconnectHandler;
		public IEthHandler Eth { get; }

		public AnkrSDKWrapper(
			IWeb3 web3Provider,
			IContractFunctions contractFunctions,
			IEthHandler eth,
			IDisconnectHandler disconnectHandler)
		{
			_web3Provider = web3Provider;
			_contractFunctions = contractFunctions;
			_disconnectHandler = disconnectHandler;
			Eth = eth;
		}

		public IContract GetContract(string contractAddress, string contractABI)
		{
			return new Contract(_web3Provider, Eth, _contractFunctions, contractAddress, contractABI);
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