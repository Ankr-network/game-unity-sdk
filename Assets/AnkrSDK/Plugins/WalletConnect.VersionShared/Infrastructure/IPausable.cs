
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IPausable
	{
		UniTask OnApplicationPause(bool pauseStatus);
	}
}