using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.Eth
{
	[RpcMethod("wallet_switchEthereumChain")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2015)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2016)]
	public class WalletSwitchEthChainData : RpcRequestListDataBase
	{
		public WalletSwitchEthChainData(params EthChain[] chains)
		{
			if (chains != null)
			{
				foreach (var chain in chains)
				{
					Add(chain);
				}
			}
		}
	}
}