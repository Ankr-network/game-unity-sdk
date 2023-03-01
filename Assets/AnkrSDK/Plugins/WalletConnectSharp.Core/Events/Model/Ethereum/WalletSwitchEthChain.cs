using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnectSharp.Core.Events.Model.Ethereum
{
	public class WalletSwitchEthChain : JsonRpcRequest
	{
		[JsonProperty("params")]
		private EthChain[] _chain;

		[JsonIgnore]
		public EthChain[] Chain => _chain;

		public WalletSwitchEthChain(params EthChain[] chain)
		{
			Method = "wallet_switchEthereumChain";
			_chain = chain;
		}
	}
}