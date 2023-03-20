using MirageSDK.Data;
using Cysharp.Threading.Tasks;

namespace MirageSDK.Core.Infrastructure
{
	public interface INetworkHelper
	{
		UniTask AddAndSwitchNetwork(EthereumNetwork network);
	}
}