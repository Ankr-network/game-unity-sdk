using System.Collections.Generic;
using AnkrSDK.Core.Implementation;
using AnkrSDK.Core.Infrastructure;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core
{
	public class AnkrSDKWrapper : IAnkrSDK
	{
		private readonly IContractFunctions _contractFunctions;
		private readonly IWalletHandler _walletHandler;
		public INetworkHelper NetworkHelper { get; }
		public IEthHandler Eth { get; }

		public AnkrSDKWrapper(
			IContractFunctions contractFunctions,
			IEthHandler eth,
			IWalletHandler disconnectHandler,
			INetworkHelper networkHelper)
		{
			_contractFunctions = contractFunctions;
			_walletHandler = disconnectHandler;
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
			return _walletHandler.Disconnect(waitForNewSession);
		}
		
		public UniTask<Dictionary<string, bool>> GetWalletsStatus()
		{
			return _walletHandler.GetWalletsStatus();
		}
	}
}