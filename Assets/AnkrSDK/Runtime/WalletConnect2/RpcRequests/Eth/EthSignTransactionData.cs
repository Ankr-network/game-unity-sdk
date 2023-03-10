using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.Eth
{
	[RpcMethod("eth_signTransaction")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2009)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2010)]
	public class EthSignTransactionData : RpcRequestListDataBase
	{
		public EthSignTransactionData(params TransactionData[] transactionDatas)
		{
			if (transactionDatas != null)
			{
				foreach (var transactionData in transactionDatas)
				{
					Add(transactionData);
				}
			}
		}
	}
}