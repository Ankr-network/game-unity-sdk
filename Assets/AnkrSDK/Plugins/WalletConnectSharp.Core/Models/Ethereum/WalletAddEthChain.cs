using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models.Ethereum
{
	public class WalletAddEthChain : JsonRpcRequest
	{
		[JsonProperty("params")] private EthChainData[] _chainData;

		[JsonIgnore] public EthChainData[] ChainData => _chainData;

		public WalletAddEthChain(EthChainData chainData) : base()
		{
			this.Method = "wallet_addEthereumChain";
			this._chainData = new[] { chainData };
		}
	}
}