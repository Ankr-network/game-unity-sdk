using System.Collections.Generic;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using Nethereum.Hex.HexTypes;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public static class EthereumNetworks
	{
		public static Dictionary<NetworkName, EthereumNetwork> Dictionary = new Dictionary<NetworkName, EthereumNetwork>
		{
			{NetworkName.Ethereum, GetSimpleNetwork(1)},
			{NetworkName.Ropsten, GetSimpleNetwork(3)},
			{NetworkName.Rinkeby, GetSimpleNetwork(4)},
			{NetworkName.Goerli, GetSimpleNetwork(5)},
			{NetworkName.Kovan, GetSimpleNetwork(42)},
			{
				NetworkName.BinanceSmartChain, new EthereumNetwork
				{
					ChainId = new HexBigInteger(56),
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
				NetworkName.BinanceSmartChain_TestNet, new EthereumNetwork
				{
					ChainId = new HexBigInteger(97),
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

		public static EthereumNetwork GetSimpleNetwork(int chainId)
		{
			return new EthereumNetwork {ChainId = new HexBigInteger(chainId)};
		}
	}
}