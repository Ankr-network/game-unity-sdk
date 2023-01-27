
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Infrastructure
{
	public interface IPausableComponent
	{
		UniTask OnApplicationPause(bool pauseStatus);
	}
}