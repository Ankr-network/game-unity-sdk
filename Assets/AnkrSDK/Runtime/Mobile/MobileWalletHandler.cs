using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Mobile
{
	public class MobileWalletHandler : IWalletHandler
	{
		private readonly WalletConnectSharp.Unity.WalletConnect _walletConnect;
 
		public MobileWalletHandler()
		{
			_walletConnect = ConnectProvider<WalletConnectSharp.Unity.WalletConnect>.GetConnect();
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			var status = new WalletsStatus
			{
				{Wallet.Unknown, false}
			};
			return UniTask.FromResult(status);
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return _walletConnect.CloseSession(waitForNewSession);
		}
	}
}