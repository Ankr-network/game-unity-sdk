using System;
using System.Collections.Generic;
using AnkrSDK.Data;
using Nethereum.Hex.HexTypes;

namespace AnkrSDK.Utils
{
	public static class EthereumNetworks
	{
		private static readonly string _ethereumMainnetName = "Mainnet";
		
		public static Dictionary<NetworkName, EthereumNetwork> Dictionary = new Dictionary<NetworkName, EthereumNetwork>
		{
			{ NetworkName.Ethereum, CreateMetamaskExistedNetwork(1, _ethereumMainnetName) },
			{ NetworkName.Ropsten, CreateMetamaskExistedNetwork(3, nameof(NetworkName.Ropsten)) },
			{ NetworkName.Rinkeby, CreateMetamaskExistedNetwork(4, nameof(NetworkName.Rinkeby)) },
			{ NetworkName.Goerli, CreateMetamaskExistedNetwork(5, nameof(NetworkName.Goerli)) },
			{ NetworkName.Kovan, CreateMetamaskExistedNetwork(42, nameof(NetworkName.Kovan)) },
			{ NetworkName.BinanceSmartChain, CreateBinanceSmartChain() },
			{ NetworkName.BinanceSmartChain_TestNet, CreateBinanceSmartChainTestNet() }
		};
		
		public static EthereumNetwork GetNetworkByName(NetworkName networkName)
		{
			if (Dictionary.ContainsKey(networkName))
			{
				return Dictionary[networkName];
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
			}
		}

		private static EthereumNetwork CreateMetamaskExistedNetwork(int chainId, string name)
		{
			return new EthereumNetwork
			{
				ChainId = new HexBigInteger(chainId),
				ChainName = name
			};
		}

		private static EthereumNetwork CreateBinanceSmartChain()
		{
			return new EthereumNetwork
			{
				ChainId = new HexBigInteger(56),
				ChainName = "Smart BNB",
				NativeCurrency = new NativeCurrency
				{
					Name = "Smart BNB",
					Symbol = "BNB",
					Decimals = 18
				},
				RpcUrls = new[] {"https://rpc.ankr.com/bsc"},
				BlockExplorerUrls = new[] {"https://bscscan.com"},
				IconUrls = new[] {"future"}
			};
		}
		
		private static EthereumNetwork CreateBinanceSmartChainTestNet()
		{
			return new EthereumNetwork
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
			};
		}
	}
}