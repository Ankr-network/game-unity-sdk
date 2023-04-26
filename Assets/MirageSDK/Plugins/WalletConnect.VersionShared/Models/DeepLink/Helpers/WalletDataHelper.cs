using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace MirageSDK.WalletConnect.VersionShared.Models.DeepLink.Helpers
{
	public static class WalletDataHelper
	{
		private static readonly Dictionary<Wallets, string> WalletNames = new Dictionary<Wallets, string>
		{
			{ Wallets.Trust, "Trust Wallet" },
			{ Wallets.MrgWallet, "MRG Wallet" }
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

		/// <summary>
		/// URL override is required for wallet not officially supported by wallet connect
		/// but still supported by our sdk like mrg wallet
		/// </summary>
		/// <param name="walletName"></param>
		/// <returns></returns>
		public static string GetOverrideUrl(string walletName)
		{
			switch (walletName)
			{
				case "MRG Wallet":
				{
					return
						"https://play-lh.googleusercontent.com/VxEmZKRr4z44UfWdx8LkjMHPfBDgUQ4k_GKvsRvrIPe377TKpBJsRzXwRvAhDrb0BEw=w240-h480-rw";
				}
			}

			return null;
		}

		public static IEnumerable<string> GetSupportedWalletNames()
		{
			var values = (Wallets[])Enum.GetValues(typeof(Wallets));
			return values.Select(v => v.GetWalletName());
		}
	}
}