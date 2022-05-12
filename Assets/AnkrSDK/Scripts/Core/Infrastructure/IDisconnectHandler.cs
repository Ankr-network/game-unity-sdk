using Cysharp.Threading.Tasks;

namespace AnkrSDK.Core.Infrastructure
{
	public interface IDisconnectHandler
	{
		UniTask Disconnect(bool waitForNewSession = true);
	}
}