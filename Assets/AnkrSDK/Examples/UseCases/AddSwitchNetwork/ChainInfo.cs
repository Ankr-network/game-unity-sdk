using System;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public static class ChainInfo
	{
		public static readonly EthChainData BscMainNet = new()
		{
			chainId = "0x38", //56
			blockExplorerUrls = new[] { "https://bscscan.com/" },
			chainName = "BSC Smart Chain",
			iconUrls = Array.Empty<string>(),
			nativeCurrency = new NativeCurrency { decimals = 18, name = "BNB", symbol = "BNB" },
			rpcUrls = new[] { "https://rpc.ankr.com/bsc" }
		};

		public static readonly EthChainData BscTestnet = new()
		{
			chainId = "0x61", //97
			blockExplorerUrls = new[] { "https://explorer.binance.org/smart-testnet" },
			chainName = "BSC Testnet",
			iconUrls = Array.Empty<string>(),
			nativeCurrency = new NativeCurrency { decimals = 18, name = "BNB", symbol = "BNB" },
			rpcUrls = new[] { "https://rpc.ankr.com/bsc_testnet_chapel" }
		};

		public static readonly EthChain BscMainNetChain = new()
		{
			chainId = "0x38" //56
		};

		public static readonly EthChain BscTestnetChain = new()
		{
			chainId = "0x61" //97
		};
	}
}