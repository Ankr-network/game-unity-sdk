
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Core.Infrastructure
{
	public interface IPausable
	{
		UniTask OnApplicationPause(bool pauseStatus);
	}
}