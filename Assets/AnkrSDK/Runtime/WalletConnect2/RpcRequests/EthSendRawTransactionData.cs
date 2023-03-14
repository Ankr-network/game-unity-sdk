using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	[RpcMethod("eth_sendRawTransaction")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2011)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2012)]
	public class EthSendRawTransactionData : RpcRequestListDataBase
	{
		public EthSendRawTransactionData(string rawTransactionString)
		{
			Add(rawTransactionString);
		}
	}
}