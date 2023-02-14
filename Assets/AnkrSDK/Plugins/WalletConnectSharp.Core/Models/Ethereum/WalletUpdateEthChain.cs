using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Models.Ethereum
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