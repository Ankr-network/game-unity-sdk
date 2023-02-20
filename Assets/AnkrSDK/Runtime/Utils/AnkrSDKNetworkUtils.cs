using System;
using AnkrSDK.Data;

namespace AnkrSDK.Utils
{
	public static class AnkrSDKNetworkUtils
	{
		public static NativeCurrency GetNativeCurrency(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.BinanceSmartChain:
					{
						return new NativeCurrency
						{
							Name = "Smart BNB", Symbol = "BNB", Decimals = 18
						};
					}
				case NetworkName.BinanceSmartChain_TestNet:
					{
						return new NativeCurrency
						{
							Name = "BNB Testnet", Symbol = "BNB", Decimals = 18
						};
					}
			}

			return null;
		}

		public static string[] GetBlockExporerUrls(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.BinanceSmartChain:
					{
						return new[]
						{
							"https://bscscan.com"
						};
					}
				case NetworkName.BinanceSmartChain_TestNet:
					{
						return new[]
						{
							"https://testnet.bscscan.com"
						};
					}
			}

			return null;
		}

		public static string[] GetIconUrls(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.BinanceSmartChain:
				case NetworkName.BinanceSmartChain_TestNet:
					{
						return new[] {"future"};
					}
			}

			return null;
		}

		public static string[] GetAnkrRPCsForSelectedNetwork(NetworkName networkName)
		{
			string rpcUrl = null;

			try
			{
				rpcUrl = GetAnkrRPCForSelectedNetwork(networkName);
				return new []
				{
					rpcUrl
				};
			}
			catch (NotSupportedException e)
			{
				return null;
			}
		}
		
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
					return "https://data-seed-prebsc-1-s1.binance.org:8545/";
				default: 
					throw new NotSupportedException();
			}
		}
	}
}