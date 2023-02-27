
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Plugins.WalletConnect.VersionShared.Infrastructure
{
	public interface IPausable
	{
		UniTask OnApplicationPause(bool pauseStatus);
	}
}