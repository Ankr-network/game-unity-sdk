using System.Collections.Generic;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Utils;
using AnkrSDK.WalletConnectSharp.Unity;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.Mobile
{
	public class MobileWalletHandler : IWalletHandler
	{
		private readonly WalletConnect _walletConnect;
 
		public MobileWalletHandler()
		{
			_walletConnect = ConnectProvider<WalletConnect>.GetWalletConnect();
		}

		public UniTask<Dictionary<string, bool>> GetWalletsStatus()
		{
			var isConnected = _walletConnect?.Session?.Accounts?.Length > 0;
			var status = new Dictionary<string, bool>
			{
				{"Metamask", isConnected}
			};
			return UniTask.FromResult(status);
		}

		public UniTask Disconnect(bool waitForNewSession = true)
		{
			return _walletConnect.CloseSession(waitForNewSession);
		}
	}
}