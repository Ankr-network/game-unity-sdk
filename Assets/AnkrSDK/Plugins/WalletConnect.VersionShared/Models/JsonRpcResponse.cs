using AnkrSDK.WalletConnect.VersionShared.Infrastructure;
using Newtonsoft.Json;

namespace AnkrSDK.WalletConnect.VersionShared.Models
{
	public class JsonRpcResponse : IEventSource, IErrorHolder
	{
		[JsonProperty]
		private JsonRpcError error;

		[JsonProperty]
		private long id;

		[JsonProperty]
		private string jsonrpc = "2.0";

		[JsonIgnore]
		public long ID => id;

		[JsonIgnore]
		public string JsonRPC => jsonrpc;

		[JsonIgnore]
		public JsonRpcError Error => error;

		[JsonIgnore]
		public bool IsError => error != null;

		[JsonIgnore]
		public string Event => "response:" + ID;

		public class JsonRpcError
		{
			[JsonProperty("code")]
			private int? code;

			[JsonProperty("data")]
			private string data;

			[JsonProperty("message")]
			private string message;

			[JsonIgnore]
			public int? Code => code;

			[JsonIgnore]
			public string Message => message;

			[JsonIgnore]
			public string Data => data;
		}
	}
}