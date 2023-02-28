using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events.Model.Ethereum
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