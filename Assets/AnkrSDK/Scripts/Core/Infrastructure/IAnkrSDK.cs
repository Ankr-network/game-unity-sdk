using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IAnkrSDK
	{
		IEthHandler Eth { get; }
		INetworkHelper NetworkHelper { get; }
		IContract GetContract(string contractAddress, string contractABI);
		IContractEventSubscriber CreateSubscriber(string wsUrl);
		UniTask<Dictionary<string, bool>> GetWalletsStatus();
	}
}
