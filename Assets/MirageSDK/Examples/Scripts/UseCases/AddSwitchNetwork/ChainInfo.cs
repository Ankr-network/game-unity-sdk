using System;
using MirageSDK.WalletConnect.VersionShared.Models.Ethereum;

namespace MirageSDK.UseCases.AddSwitchNetwork
{
	public static class ChainInfo
	{
		public static readonly EthChainData BscMainNet = new EthChainData()
		{
			chainId = "0x38", //56
			blockExplorerUrls = new[] { "https://bscscan.com/" },
			chainName = "BSC Smart Chain",
			iconUrls = Array.Empty<string>(),
			nativeCurrency = new NativeCurrency { decimals = 18, name = "BNB", symbol = "BNB" },
			rpcUrls = new[] { "https://rpc.ankr.com/bsc" }
		};

		public static readonly EthChainData BscTestnet = new EthChainData()
		{
			chainId = "0x61", //97
			blockExplorerUrls = new[] { "https://explorer.binance.org/smart-testnet" },
			chainName = "BSC Testnet",
			iconUrls = Array.Empty<string>(),
			nativeCurrency = new NativeCurrency { decimals = 18, name = "BNB", symbol = "BNB" },
			rpcUrls = new[] { "https://rpc.ankr.com/bsc_testnet_chapel" }
		};

		public static readonly EthChain BscMainNetChain = new EthChain()
		{
			chainId = "0x38" //56
		};

		public static readonly EthChain BscTestnetChain = new EthChain()
		{
			chainId = "0x61" //97
		};
	}
}