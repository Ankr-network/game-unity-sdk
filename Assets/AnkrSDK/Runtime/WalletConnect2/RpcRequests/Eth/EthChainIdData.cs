using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.Eth
{
	[RpcMethod("wallet_updateEthereumChain")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2017)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2018)]
	public class EthChainIdData : RpcRequestListDataBase
	{
	}
}