using AnkrSDK.Core.Infrastructure;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Mobile
{
	public class MobileDisconnectHandler : IDisconnectHandler
	{
		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return WalletConnect.CloseSession(waitForNewSession);
		}
	}
}