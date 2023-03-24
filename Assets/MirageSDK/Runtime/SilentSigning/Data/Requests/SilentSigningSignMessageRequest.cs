using MirageSDK.WalletConnect.VersionShared.Models;
using Newtonsoft.Json;

namespace MirageSDK.SilentSigning.Data.Requests
{
	public class SilentSigningSignMessageRequest : JsonRpcRequest
	{
		public class SilentSigningSignMessageRequestParams
		{
			[JsonProperty("message")] 
			public string Message;

			[JsonProperty("address")]
			public string Address;
		}
		
		[JsonProperty("params")] private SilentSigningSignMessageRequestParams[] _parameters;

		[JsonIgnore] public SilentSigningSignMessageRequestParams[] Parameters => _parameters;

		public SilentSigningSignMessageRequest(string address, string message)
		{
			Method = "wallet_silentSignMessage";
			_parameters = new[]
			{
				new SilentSigningSignMessageRequestParams()
				{
					Message = message,
					Address = address
				}
			};
		}
	}
}