using JetBrains.Annotations;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;

namespace AnkrSDK.Core.Infrastructure
{
	public class EthereumNetwork
	{
		[JsonProperty("chainId")]
		public HexBigInteger ChainId { get; set; }
		[JsonProperty("chainName")]
		public string ChainName { get; set; }
		[JsonProperty("nativeCurrency")]
		public NativeCurrency NativeCurrency { get; set; }
		[CanBeNull]
		[JsonProperty("rpcUrls")]
		public string[] RpcUrls { get; set; }
		[CanBeNull]
		[JsonProperty("blockExplorerUrls")]
		public string[] BlockExplorerUrls { get; set; }
		[CanBeNull]
		[JsonProperty("iconUrls")]
		public string[] IconUrls { get; set; }
	}

	public class NativeCurrency
	{
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("symbol")]
		public string Symbol;
		[JsonProperty("decimals")]
		public int Decimals;
	}
}