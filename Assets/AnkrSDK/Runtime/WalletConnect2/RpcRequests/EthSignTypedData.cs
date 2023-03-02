using AnkrSDK.WalletConnect.VersionShared.Models.Ethereum.Types;
using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.WalletConnect2.RpcRequests
{
	[RpcMethod("eth_signTypedData")]
	[RpcRequestOptions(Clock.SIX_HOURS, false, 2007)]
	[RpcResponseOptions(Clock.SIX_HOURS, false, 2008)]
	public class EthSignTypedData<T> : RpcRequestListDataBase
	{
		public EthSignTypedData(string address, T data, EIP712Domain domain)
		{
			var typeData = new EvmTypedData<T>(data, domain);
			var encodedTypeData = JsonConvert.SerializeObject(typeData);
			Add(address);
			Add(encodedTypeData);
		}
	}
}