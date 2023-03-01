using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect.VersionShared.Models
{
	public class GenericJsonRpcRequest : JsonRpcRequest
	{
		[JsonProperty("params")]
		[JsonConverter(typeof (RpcParametersJsonConverter))]
		public object RawParameters { get; private set; }
		
		public GenericJsonRpcRequest(RpcRequestMessage message)
		{
			id = (long)message.Id;
			Method = message.Method;
			RawParameters = message.RawParameters;
		}

		public GenericJsonRpcRequest(long overrideId, RpcRequestMessage message)
		{
			id = overrideId;
			Method = message.Method;
			RawParameters = message.RawParameters;
		}
	}
}