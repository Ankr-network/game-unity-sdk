using System.Threading.Tasks;
using MirageSDK.Data;

namespace MirageSDK.Core.Infrastructure
{
	public interface INetworkHelper
	{
		Task AddAndSwitchNetwork(EthereumNetwork network);
	}
}