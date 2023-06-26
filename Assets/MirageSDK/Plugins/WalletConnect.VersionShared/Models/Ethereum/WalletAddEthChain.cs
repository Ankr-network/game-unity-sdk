using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class WalletAddEthChain : JsonRpcRequest
	{
		[JsonProperty("params")]
		private EthChainData[] _params;

		[JsonIgnore] public EthChainData[] Params => _params;

		public WalletAddEthChain(EthChainData chainData)
		{
			Method = "wallet_addEthereumChain";
			_params = new[]
			{
				chainData
			};
		}
	}
}