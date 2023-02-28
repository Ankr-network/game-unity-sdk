namespace AnkrSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthChainData : EthChain
	{
		public string[] blockExplorerUrls;
		public string chainName;
		public string[] iconUrls;
		public NativeCurrency nativeCurrency;
		public string[] rpcUrls;

		public EthUpdateChainData ToEthUpdate()
		{
			return new EthUpdateChainData
			{
				chainId = chainId, chainName = chainName, rpcUrl = rpcUrls != null && rpcUrls.Length > 0 ? rpcUrls[0] : null, nativeCurrency = nativeCurrency,
				blockExplorerUrl = blockExplorerUrls != null && blockExplorerUrls.Length > 0 ? blockExplorerUrls[0] : null
			};
		}
	}
}