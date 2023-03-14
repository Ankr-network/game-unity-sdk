using AnkrSDK.WalletConnect.VersionShared.Models;
using AnkrSDK.WalletConnect2.RpcRequests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnkrSDK.WalletConnect2.RpcResponses
{
	public class GenericResponseData : RpcRequestDataBase
	{
		[JsonProperty("result")]
		public JToken Result { get; private set; }

		public GenericJsonRpcResponse ToGenericRpcResponse()
		{
			return new GenericJsonRpcResponse(Result);
		}
	}
}