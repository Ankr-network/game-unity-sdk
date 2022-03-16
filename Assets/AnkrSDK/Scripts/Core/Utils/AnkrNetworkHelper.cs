using System;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Core.Data;
using UnityEngine;

namespace AnkrSDK.Core.Utils
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
						AnkrSDKHelper.GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChain));
					break;
				case NetworkName.BinanceSmartChainTestNet:
					AddAndSwitchCustomNetwork(
						AnkrSDKHelper.GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChainTestNet));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}
		}

		public static void AddAndSwitchCustomNetwork(string url)
		{
			Application.OpenURL(url);
		}
	}
}