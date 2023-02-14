using System;
using AnkrSDK.Data;

namespace AnkrSDK.Utils
{
	public static class AnkrSDKFactoryHelper
	{
		public static string GetAnkrRPCForSelectedNetwork(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.Mainnet:
					return "https://rpc.ankr.com/eth";
				case NetworkName.Rinkeby:
					return "https://rpc.ankr.com/eth_rinkeby";
				case NetworkName.Goerli:
					return "https://rpc.ankr.com/eth_goerli";
				case NetworkName.Ropsten:
					return "https://rpc.ankr.com/eth_ropsten";
				case NetworkName.Polygon:
					return "https://rpc.ankr.com/polygon";
				case NetworkName.Polygon_Mumbai:
					return "https://rpc.ankr.com/polygon_mumbai";
				case NetworkName.BinanceSmartChain:
					return "https://rpc.ankr.com/bsc";
				case NetworkName.BinanceSmartChain_TestNet:
					default: throw new NotSupportedException();
			}
		}
	}
}