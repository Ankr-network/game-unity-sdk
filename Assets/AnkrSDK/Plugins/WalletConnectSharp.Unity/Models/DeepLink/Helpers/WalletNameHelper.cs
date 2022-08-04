using System.Collections.Generic;

namespace AnkrSDK.WalletConnectSharp.Unity.Models.DeepLink.Helpers
{
	public static class WalletNameHelper
	{
		private static readonly Dictionary<Wallets, string> WalletNames = new Dictionary<Wallets, string>
		{
			{ Wallets.Trust, "Trust Wallet" }
		};

		public static string GetWalletName(this Wallets wallets)
		{
			return WalletNames.ContainsKey(wallets) ? WalletNames[wallets] : wallets.ToString();
		}
	}
}