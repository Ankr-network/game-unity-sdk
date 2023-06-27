using Newtonsoft.Json;

namespace MirageSDK.WalletConnect.VersionShared.Models.Ethereum
{
	public class EthGetBalance : JsonRpcRequest
	{
		[JsonProperty("params")]
		private object[] _parameters;

		[JsonIgnore]
		public object[] Parameters => _parameters;

		public EthGetBalance(string address) : base()
		{
			this.Method = "eth_getBalance";
			this._parameters = new object[]
			{
				address,
				"latest",
			};
		}
	}
}