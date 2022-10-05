using System.Threading.Tasks;
using AnkrSDK.Data;

namespace AnkrSDK.Core.Infrastructure
{
	public interface INetworkHelper
	{
		Task AddAndSwitchNetwork(EthereumNetwork network);
	}
}