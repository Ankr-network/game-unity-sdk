
using Newtonsoft.Json;

namespace MirageSDK.WebGL.DTO.JsonRpc
{
	public class EthCallRequest : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthCallRequest(TransactionData transactionData) : base()
		{
			this.Method = "eth_sendTransaction";
			this._parameters = new object[]
			{
				transactionData,
				"latest",
			};
		}
	}
}