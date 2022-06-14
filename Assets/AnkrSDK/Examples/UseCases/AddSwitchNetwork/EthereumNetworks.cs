using System;
using System.Collections.Generic;
using AnkrSDK.Data;
using Nethereum.Hex.HexTypes;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public static class EthereumNetworks
	{
		private static readonly string _ethereumMainnetName = "Mainnet";
		
		private static Dictionary<NetworkName, EthereumNetwork> _dictionary = new Dictionary<NetworkName, EthereumNetwork>
		{
			{ NetworkName.Ethereum, CreateMetamaskExistedNetwork(1, _ethereumMainnetName) },
			{ NetworkName.Ethereum_Ropsten, CreateMetamaskExistedNetwork(3, nameof(NetworkName.Ethereum_Ropsten)) },
			{ NetworkName.Ethereum_Rinkeby, CreateMetamaskExistedNetwork(4, nameof(NetworkName.Ethereum_Rinkeby)) },
			{ NetworkName.Ethereum_Goerli, CreateMetamaskExistedNetwork(5, nameof(NetworkName.Ethereum_Goerli)) },
			{ NetworkName.Ethereum_Kovan, CreateMetamaskExistedNetwork(42, nameof(NetworkName.Ethereum_Kovan)) },
			{ NetworkName.BinanceSmartChain, CreateBinanceSmartChain() },
			{ NetworkName.BinanceSmartChain_TestNet, CreateBinanceSmartChainTestNet() }
		};
		
		public static EthereumNetwork GetNetworkByName(NetworkName networkName)
		{
			if (_dictionary.ContainsKey(networkName))
			{
				return _dictionary[networkName];
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
				RpcUrls = new[] {"https://bsc-dataseed.binance.org/"},
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