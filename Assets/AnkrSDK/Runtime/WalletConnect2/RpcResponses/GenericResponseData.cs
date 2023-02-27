using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;
using AnkrSDK.Runtime.WalletConnect2.RpcRequests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnkrSDK.Runtime.WalletConnect2.RpcResponses
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