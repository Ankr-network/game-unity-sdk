using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	[RpcMethod("eth_sign")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2005)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2006)]
	public class EthSignRequestData : RpcRequestListDataBase
	{
		public EthSignRequestData(string address, string hexData)
		{
			Add(address);
			Add(hexData);
		}
	}
}