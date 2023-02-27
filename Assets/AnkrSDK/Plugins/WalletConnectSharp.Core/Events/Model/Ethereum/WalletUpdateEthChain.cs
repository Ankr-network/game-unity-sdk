using AnkrSDK.Plugins.WalletConnect.VersionShared.Models;
using AnkrSDK.Plugins.WalletConnect.VersionShared.Models.Ethereum;
using Newtonsoft.Json;

namespace AnkrSDK.Plugins.WalletConnectSharp.Core.Events.Model.Ethereum
{
	public class WalletUpdateEthChain : JsonRpcRequest
	{
		[JsonProperty("params")]
		private EthUpdateChainData[] _params;

		public WalletUpdateEthChain(EthUpdateChainData chainData)
		{
			Method = "wallet_updateEthereumChain";
			_params = new[]
			{
				chainData
			};
		}
	}
}