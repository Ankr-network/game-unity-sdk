using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events.Model.Ethereum
{
	public class WalletUpdateEthChain : JsonRpcRequest
	{
		[JsonProperty("params")]
		private EthUpdateChainData[] _params;

		public WalletUpdateEthChain(EthUpdateChainData chainData)
		{
			Method = "wallet_updateChain";
			_params = new[]
			{
				chainData
			};
		}
	}
}