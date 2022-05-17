using System.Collections.Generic;
using AnkrSDK.Core.Data;
using AnkrSDK.Core.Infrastructure;
using Nethereum.Hex.HexTypes;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public static class EthereumNetworks
	{
		public static Dictionary<NetworkName, EthereumNetwork> Dictionary = new Dictionary<NetworkName, EthereumNetwork>
		{
			{
				NetworkName.BinanceSmartChain, new EthereumNetwork
				{
					ChainId = new HexBigInteger(38),
					ChainName = "Smart BNB",
					NativeCurrency = new NativeCurrency
					{
						Name = "Smart BNB",
						Symbol = "BNB",
						Decimals = 18
					},
					RpcUrls = new[] {"https://bsc-dataseed.binance.org/"},
					BlockExplorerUrls = new[] {"https://bscscan.com"},
					IconUrls = new[] {"future"}
				}
			},
			{
				NetworkName.BinanceSmartChainTestNet, new EthereumNetwork
				{
					ChainId = new HexBigInteger(61),
					ChainName = "Smart Chain - Testnet",
					NativeCurrency = new NativeCurrency
					{
						Name = "BNB Testnet",
						Symbol = "BNB",
						Decimals = 18
					},
					RpcUrls = new[] {"https://data-seed-prebsc-1-s1.binance.org:8545/"},
					BlockExplorerUrls = new[] {"https://testnet.bscscan.com"},
					IconUrls = new[] {"future"}
				}
			}
		};
	}
}