using System;
using MirageSDK.Core.Data;
using MirageSDK.Core.Infrastructure;
using UnityEngine;

namespace MirageSDK.Core.Utils
{
	public static class MirageNetworkHelper
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
						MirageSDKHelper.GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChain));
					break;
				case NetworkName.BinanceSmartChainTestNet:
					AddAndSwitchCustomNetwork(
						MirageSDKHelper.GetURLFromNetworkNameEnum(NetworkName.BinanceSmartChainTestNet));
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