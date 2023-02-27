using AnkrSDK.Plugins.WalletConnectSharp.Core.Models;
using AnkrSDK.Runtime.WalletConnect2.RpcRequests;

namespace AnkrSDK.Runtime.WalletConnect2.RpcResponses
{
	public class GenericResponseData : RpcRequestDataBase
	{
		public GenericJsonRpcResponse ToGenericRpcResponse()
		{
			var result = new GenericJsonRpcResponse();

			
			return result;

		}
	}
}