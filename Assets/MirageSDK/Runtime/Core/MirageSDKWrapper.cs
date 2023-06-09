using System.IO;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data;

namespace MirageSDK.Core
{
	public class MirageSDKWrapper : IMirageSDK
	{
		private readonly IContractFunctions _contractFunctions;
		public IEthHandler Eth { get; }
		public ISilentSigningHandler SilentSigningHandler { get; }

		public MirageSDKWrapper(
			IContractFunctions contractFunctions,
			IEthHandler eth,
			ISilentSigningHandler silentSigningHandler = null
		)
		{
			_contractFunctions = contractFunctions;
			SilentSigningHandler = silentSigningHandler;
			Eth = eth;
		}

		public IContract GetContract(string contractAddress, string contractABI)
		{
			return new Contract(Eth, _contractFunctions, contractAddress, contractABI);
		}

		public IContract GetContract(ContractInformationSO contractInfo)
		{
			if (!contractInfo.IsValid)
			{
				throw new InvalidDataException($"Invalid contract data: {contractInfo}");
			}

			return new Contract(Eth, _contractFunctions, contractInfo.ContractAddress, contractInfo.ABI);
		}

		public IContractEventSubscriber CreateSubscriber(string wsUrl)
		{
			return new ContractEventSubscriber(wsUrl);
		}
	}
}