using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	[RpcMethod("eth_sendTransaction")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2003)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2004)]
	public class EthSendTransactionData : RpcRequestListDataBase
	{
		public EthSendTransactionData(params TransactionData[] transactionDatas)
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