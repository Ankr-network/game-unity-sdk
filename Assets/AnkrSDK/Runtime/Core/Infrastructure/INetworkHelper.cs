using AnkrSDK.Data;
using AnkrSDK.WalletConnectSharp.Core.Models.Ethereum;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface INetworkHelper
	{
		UniTask AddAndSwitchNetwork(EthChainData chain);
	}
}