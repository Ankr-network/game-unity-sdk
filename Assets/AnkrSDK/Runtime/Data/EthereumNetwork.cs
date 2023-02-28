using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using JetBrains.Annotations;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;

namespace AnkrSDK.Data
{
	public class EthereumNetwork
	{
		[JsonProperty("chainId")]
		public HexBigInteger ChainId { get; set; }
		[JsonProperty("chainName")]
		public string ChainName { get; set; }
		[JsonProperty("nativeCurrency")]
		public NativeCurrency NativeCurrency { get; set; }
		[CanBeNull, JsonProperty("rpcUrls")]
		public string[] RpcUrls { get; set; }
		[CanBeNull, JsonProperty("blockExplorerUrls")]
		public string[] BlockExplorerUrls { get; set; }
		[CanBeNull, JsonProperty("iconUrls")]
		public string[] IconUrls { get; set; }

		public EthChainData ToEthChainData()
		{
			var result = new EthChainData
			{
				chainId = "0x" + ChainId,
				chainName = ChainName,
				iconUrls = IconUrls,
				nativeCurrency = new WalletConnect.VersionShared.Models.Ethereum.NativeCurrency
				{
					name = NativeCurrency.Name,
					symbol = NativeCurrency.Symbol,
					decimals = NativeCurrency.Decimals
				},
				rpcUrls = RpcUrls,
			};

			return result;
		}
	}
}