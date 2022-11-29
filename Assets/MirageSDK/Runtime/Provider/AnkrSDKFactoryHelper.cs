using System;
using MirageSDK.Data;

namespace MirageSDK.Provider
{
	public static class MirageSDKFactoryHelper
	{
		public static string GetMirageRPCForSelectedNetwork(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.Ethereum:
					return "https://rpc.Mirage.com/eth";
				case NetworkName.Rinkeby:
					return "https://rpc.Mirage.com/eth_rinkeby";
				case NetworkName.Goerli:
					return "https://rpc.Mirage.com/eth_goerli";
				case NetworkName.Ropsten:
					return "https://rpc.Mirage.com/eth_ropsten";
				case NetworkName.Polygon:
					return "https://rpc.Mirage.com/polygon";
				case NetworkName.Polygon_Mumbai:
					return "https://rpc.Mirage.com/polygon_mumbai";
				case NetworkName.BinanceSmartChain:
					return "https://rpc.Mirage.com/bsc";
				case NetworkName.BinanceSmartChain_TestNet:
					default: throw new NotSupportedException();
			}
		}
	}
}