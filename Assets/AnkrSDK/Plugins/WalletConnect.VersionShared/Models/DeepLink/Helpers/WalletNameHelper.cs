using System;
using System.Collections.Generic;
using System.Linq;

namespace AnkrSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers
{
	public static class WalletNameHelper
	{
		private static readonly Dictionary<Wallets, string> WalletNames = new Dictionary<Wallets, string>
		{
			{ Wallets.Trust, "Trust Wallet" }
		};

		/// <summary>
		/// Get Wallet Name is it described in WalletConnect Registry
		/// </summary>
		/// <param name="walletEnumValue"></param>
		/// <returns></returns>
		public static string GetWalletName(this Wallets walletEnumValue)
		{
			return WalletNames.ContainsKey(walletEnumValue)
				? WalletNames[walletEnumValue]
				: walletEnumValue.ToString();
		}

		public static IEnumerable<string> GetSupportedWalletNames()
		{
			var values = (Wallets[])Enum.GetValues(typeof(Wallets));
			return values.Select(v => v.GetWalletName());
		}
	}
}