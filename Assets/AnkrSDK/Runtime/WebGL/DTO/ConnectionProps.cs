using AnkrSDK.Data;
using Newtonsoft.Json;

namespace AnkrSDK.WebGL.DTO
{
	public class ConnectionProps
	{
		[JsonProperty]
		public string wallet;
		[JsonProperty]
		public EthereumNetwork chain;
	}
}