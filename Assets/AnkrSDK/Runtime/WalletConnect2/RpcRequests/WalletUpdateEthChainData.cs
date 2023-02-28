using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	[RpcMethod("wallet_updateEthereumChain")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2017)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2018)]
	public class WalletUpdateEthChainData : RpcRequestListDataBase
	{
		public WalletUpdateEthChainData(EthUpdateChainData chainData)
		{
			Add(chainData);
		}
	}
}