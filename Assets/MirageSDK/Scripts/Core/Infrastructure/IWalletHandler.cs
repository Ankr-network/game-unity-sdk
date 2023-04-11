using MirageSDK.Data;
using Cysharp.Threading.Tasks;

namespace MirageSDK.Core.Infrastructure
{
	public interface IWalletHandler
	{
		UniTask Disconnect(bool waitForNewSession = true);
		UniTask<WalletsStatus> GetWalletsStatus();
	}
}