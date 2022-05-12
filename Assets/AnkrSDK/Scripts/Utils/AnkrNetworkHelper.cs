using System;
using AnkrSDK.Data;
using UnityEngine;

namespace AnkrSDK.Utils
{
	public static class AnkrNetworkHelper
	{
		public static void AddAndSwitchNetwork(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.Ethereum:
					break;
				case NetworkName.EthereumRinkebyTestNet:
					break;
				case NetworkName.BinanceSmartChain:
					AddAndSwitchCustomNetwork(
						GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChain));
					break;
				case NetworkName.BinanceSmartChainTestNet:
					AddAndSwitchCustomNetwork(
						GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChainTestNet));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}
		}

		private static void AddAndSwitchCustomNetwork(string url)
		{
			Application.OpenURL(url);
		}

		private static string GetURLFromNetworkNameEnum(NetworkName networkName)
		{
			var url = "";
			
			switch (networkName)
			{
				case NetworkName.Ethereum:
					break;
				case NetworkName.EthereumRinkebyTestNet:
					break;
				case  NetworkName.BinanceSmartChain:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc";
					break;
				case  NetworkName.BinanceSmartChainTestNet:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc_test";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}

			return url;
		} 
	}
}