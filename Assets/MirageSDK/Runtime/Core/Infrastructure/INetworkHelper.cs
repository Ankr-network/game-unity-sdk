using Cysharp.Threading.Tasks;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;

namespace MirageSDK.Core.Infrastructure
{
	public interface INetworkHelper
	{
		UniTask AddAndSwitchNetwork(EthChainData chain);
	}
}