using System;
using AnkrSDK.Data;

namespace AnkrSDK.Provider
{
	public static class AnkrSDKFactoryHelper
	{
		public static string GetAnkrRPCForSelectedNetwork(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.Ethereum:
					return "https://rpc.ankr.com/eth";
				case NetworkName.Ethereum_Rinkeby:
					return "https://rpc.ankr.com/eth_rinkeby";
				case NetworkName.Ethereum_Goerli:
					return "https://rpc.ankr.com/eth_goerli";
				case NetworkName.Ethereum_Ropsten:
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