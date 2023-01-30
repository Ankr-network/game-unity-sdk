
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Core.Infrastructure
{
	public interface IPausableComponent
	{
		UniTask OnApplicationPause(bool pauseStatus);
	}
}