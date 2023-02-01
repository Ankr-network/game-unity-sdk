using System;
using AnkrSDK.WalletConnectSharp.Core.Models;
using Cysharp.Threading.Tasks;

namespace AnkrSDK.WalletConnectSharp.Unity
{
	public partial class WalletConnect
	{
		public class WalletConnectTestWrapper : IDisposable
		{
			private readonly WalletConnect _walletConnect;

			public WalletConnectTestWrapper(WalletConnect walletConnect)
			{
				_walletConnect = walletConnect;
			}
			
			public void InitializeUnitySession()
			{
				_walletConnect.InitializeUnitySession();
			}

			public UniTask<WCSessionData> Connect()
			{
				return _walletConnect.Connect();
			}

			public void Dispose()
			{
				_walletConnect?.Dispose();
			}
		}
	}
}