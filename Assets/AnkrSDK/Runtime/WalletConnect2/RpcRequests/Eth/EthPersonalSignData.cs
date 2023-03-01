using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests.Eth
{
	[RpcMethod("personal_sign")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2001)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2002)]
	public class EthPersonalSignData : RpcRequestListDataBase
	{
		public EthPersonalSignData (string address, string hexData, string password = "")
		{
			Add(address);
			Add(hexData);
			Add(password);
		}
	}
}