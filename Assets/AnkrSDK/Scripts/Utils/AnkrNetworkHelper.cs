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
				case NetworkName.Ethereum_Rinkeby_TestNet:
					break;
				case NetworkName.BinanceSmartChain:
					AddAndSwitchCustomNetwork(
						GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChain));
					break;
				case NetworkName.BinanceSmartChain_TestNet:
					AddAndSwitchCustomNetwork(
						GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChain_TestNet));
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
				case NetworkName.Ethereum_Rinkeby_TestNet:
					break;
				case  NetworkName.BinanceSmartChain:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc";
					break;
				case  NetworkName.BinanceSmartChain_TestNet:
					url = "https://metamask.app.link/dapp/change-network-mirage.surge.sh?network=bsc_test";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}

			return url;
		} 
	}
}