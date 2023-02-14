using System;
using System.Collections.Generic;
using AnkrSDK.Data;
using Nethereum.Hex.HexTypes;

namespace AnkrSDK.Utils
{
	public static class EthereumNetworks
	{
		private static readonly string _ethereumMainnetName = "Mainnet";

		private static readonly Dictionary<NetworkName, EthereumNetwork> Dictionary =
			new Dictionary<NetworkName, EthereumNetwork>();
		public static IEnumerable<NetworkName> AllAddedNetworks => Dictionary.Keys;

		static EthereumNetworks()
		{
			var networksToAdd = new NetworkName[]
			{
				NetworkName.Mainnet,
				NetworkName.Ropsten,
				NetworkName.Rinkeby,
				NetworkName.Goerli,
				NetworkName.Kovan
			};
			foreach (var networkName in networksToAdd)
			{
				var ethereumNetwork =
					CreateMetamaskExistedNetwork(networkName.GetNetworkChainId(), networkName);
				Dictionary.Add(networkName, ethereumNetwork);
			}
			
			Dictionary.Add(NetworkName.BinanceSmartChain, CreateBinanceSmartChain());
			Dictionary.Add(NetworkName.BinanceSmartChain_TestNet, CreateBinanceSmartChainTestNet());
		}
		
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

		private static EthereumNetwork CreateMetamaskExistedNetwork(int chainId, NetworkName networkName)
		{
			return new EthereumNetwork
			{
				ChainId = new HexBigInteger(chainId),
				ChainName = networkName.ToString()
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