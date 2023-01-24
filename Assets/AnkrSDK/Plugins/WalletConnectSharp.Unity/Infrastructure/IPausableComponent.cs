using System.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity.Infrastructure
{
	public interface IPausableComponent
	{
		Task OnApplicationPause(bool pauseStatus);
	}
}