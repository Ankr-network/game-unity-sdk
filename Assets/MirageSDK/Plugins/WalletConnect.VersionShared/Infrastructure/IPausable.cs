
using Cysharp.Threading.Tasks;

namespace MirageSDK.WalletConnect.VersionShared.Infrastructure
{
	public interface IPausable
	{
		UniTask OnApplicationPause(bool pauseStatus);
	}
}