using AnkrSDK.Data;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface INetworkHelper
	{
		UniTask AddAndSwitchNetwork(EthereumNetwork network);
	}
}