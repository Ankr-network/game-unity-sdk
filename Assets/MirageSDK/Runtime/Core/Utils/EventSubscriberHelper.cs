using System.Text;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace MirageSDK.Core.Utils
{
	public static class EventSubscriberHelper
	{
		public static RpcStreamingResponseMessage DeserializeMessage(byte[] message)
		{
			var messageJson = Encoding.UTF8.GetString(message);
			return JsonConvert.DeserializeObject<RpcStreamingResponseMessage>(messageJson);
		}
		
		public static string RpcRequestToString(RpcRequest rpcRequest)
		{
			var reqMsg = new RpcRequestMessage(rpcRequest.Id,
				rpcRequest.Method,
				rpcRequest.RawParameters);

			return JsonConvert.SerializeObject(reqMsg);
		}
	}
}