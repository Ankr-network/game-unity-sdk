using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Mobile
{
	public class MobileDisconnectHandler : IDisconnectHandler
	{
		private readonly WalletConnect _walletConnect;

		public MobileDisconnectHandler()
		{
			_walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return _walletConnect.CloseSession(waitForNewSession);
		}
	}
}