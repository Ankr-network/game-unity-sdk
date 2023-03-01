using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.Eth
{
	[RpcMethod("wallet_addEthereumChain")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2013)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2014)]
	public class WalletAddEthChainData : RpcRequestListDataBase
	{
		public WalletAddEthChainData(EthChainData chainData)
		{
			Add(chainData);
		}
	}
}