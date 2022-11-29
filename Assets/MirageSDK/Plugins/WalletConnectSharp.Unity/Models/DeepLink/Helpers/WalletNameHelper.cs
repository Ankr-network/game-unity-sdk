using System;
using System.Collections.Generic;
using System.Linq;

namespace MirageSDK.WalletConnectSharp.Unity.Models.DeepLink.Helpers
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
		/// <param name="wallets"></param>
		/// <returns></returns>
		public static string GetWalletName(this Wallets wallets)
		{
			return WalletNames.ContainsKey(wallets)
				? WalletNames[wallets]
				: wallets.ToString();
		}

		public static IEnumerable<string> GetSupportedWalletNames()
		{
			var values = (Wallets[])Enum.GetValues(typeof(Wallets));
			return values.Select(v => v.GetWalletName());
		}
	}
}