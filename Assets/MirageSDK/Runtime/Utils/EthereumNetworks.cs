using System;
using System.Collections.Generic;
using MirageSDK.Data;
using Nethereum.Hex.HexTypes;

namespace MirageSDK.Utils
{
	public static class EthereumNetworks
	{
		private static readonly Dictionary<NetworkName, EthereumNetwork> Dictionary =
			new Dictionary<NetworkName, EthereumNetwork>();

		public static IEnumerable<NetworkName> AllAddedNetworks => Dictionary.Keys;

		static EthereumNetworks()
		{
			foreach (NetworkName networkName in Enum.GetValues(typeof(NetworkName)))
			{
				var ethereumNetwork =
					CreateMetamaskExistedNetwork(networkName);
				Dictionary.Add(networkName, ethereumNetwork);
			}
		}

		public static EthereumNetwork GetNetworkByName(NetworkName networkName)
		{
			if (Dictionary.ContainsKey(networkName))
			{
				return Dictionary[networkName];
			}

			throw new ArgumentOutOfRangeException(nameof(networkName), networkName, null);
		}

		private static EthereumNetwork CreateMetamaskExistedNetwork(NetworkName networkName)
		{
			return new EthereumNetwork
			{
				ChainId = new HexBigInteger((int)networkName), 
				ChainName = GetChainName(networkName), 
				NativeCurrency = MirageSDKNetworkUtils.GetNativeCurrency(networkName), 
				RpcUrls = MirageSDKNetworkUtils.GetAnkrRPCsForSelectedNetwork(networkName),
				BlockExplorerUrls = MirageSDKNetworkUtils.GetBlockExporerUrls(networkName), 
				IconUrls = MirageSDKNetworkUtils.GetIconUrls(networkName)
			};
		}

		private static string GetChainName(NetworkName networkName)
		{
			switch (networkName)
			{
				case NetworkName.BinanceSmartChain:
					return "Smart BNB";
				case NetworkName.BinanceSmartChain_TestNet:
					return "Smart Chain - Testnet";
				default:
					return networkName.ToString();
			}
		}
	}
}