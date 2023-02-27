using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace AnkrSDK.Runtime.WalletConnect2.RpcRequests
{
	[RpcMethod("eth_sign")]
	[RpcRequestOptions(Clock.ONE_DAY, false, 1108)]
	public class EthSignRequestData : RpcRequestDataBase
	{
		[JsonProperty("address")]
		public string Address { get; private set; }
		
		[JsonProperty("hexData")]
		public string HexData { get; private set; }

		public EthSignRequestData(string address, string hexData)
		{
			Address = address;
			HexData = hexData;
		}
	}
}