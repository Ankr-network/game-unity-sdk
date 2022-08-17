using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IWalletHandler
	{
		UniTask Disconnect(bool waitForNewSession = true);
		UniTask<Dictionary<string, bool>> GetWalletsStatus();
	}
}