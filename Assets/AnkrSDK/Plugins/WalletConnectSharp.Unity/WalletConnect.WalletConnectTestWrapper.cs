using System;
using AnkrSDK.WalletConnectSharp.Core;
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
				_walletConnect.InitializeSession();
			}

			public void Dispose()
			{
				_walletConnect?.Dispose();
			}
		}
	}
}