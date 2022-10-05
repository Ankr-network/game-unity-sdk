using AnkrSDK.Data;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IWalletHandler
	{
		UniTask Disconnect(bool waitForNewSession = true);
		UniTask<WalletsStatus> GetWalletsStatus();
	}
}