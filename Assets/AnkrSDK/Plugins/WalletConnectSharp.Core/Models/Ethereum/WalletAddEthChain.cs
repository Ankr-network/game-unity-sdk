using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models.Ethereum
{
	public class WalletAddEthChain : JsonRpcRequest
	{
		[JsonProperty("params")] 
		private EthChainData[] _params;

		[JsonIgnore] public EthChainData[] Params => _params;

		public WalletAddEthChain(EthChainData chainData) : base()
		{
			this.Method = "wallet_addEthereumChain";
			this._params = new[] { chainData };
		}
	}
}