using AnkrSDK.Core.Implementation;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.SilentSigning
{
	[RpcMethod("wallet_silentSendTransaction")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 3007)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 3008)]
	public class SilentSigningTransactionRequestData : RpcRequestListDataBase
	{
		public SilentSigningTransactionRequestData(SilentTransactionData transactionData)
		{
			Add(transactionData);
		}
	}
}